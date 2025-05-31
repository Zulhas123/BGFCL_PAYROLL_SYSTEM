let _dataTable = undefined;
$(document).ready(function () {
    const now = new Date();
    now.setMonth(now.getMonth() - 1); // Go to previous month

    const monthString = now.toISOString().slice(0, 7); // YYYY-MM
    $('#monthPicker').val(monthString); // Set month input

    const previousMonthId = now.getFullYear().toString() + String(now.getMonth() + 1).padStart(2, '0');
    getAttendanceList(10, previousMonthId); // Load data for previous month
    loadInitialData();
    $('#isExcel').on('change', function () {
        if ($(this).is(':checked')) {
            $('#report_button')
                .text('Export To Excel')                     // Change text
                .css('background-color', 'yellow') // Set background color to yellow
                .css('color', 'black');            // Optional: change text color for better contrast
        } else {
            $('#report_button')
                .text('Report To PDF')                    // Change text back
                .css('background-color', '')       // Reset background color to default
                .css('color', '');                 // Reset text color
        }
    });
    $('#loadAttendance').on('click', function () {
        const selectedMonth = $('#monthPicker').val(); // "YYYY-MM"
        if (selectedMonth) {
            const [year, month] = selectedMonth.split("-");
            const monthId = year + month.padStart(2, '0');
            getAttendanceList(10, monthId);
        }
    });
});



function getAttendanceList(pageLength, monthId) {
    $.ajax({
        url: `/api/Users/GetAttendanceByMonthId?monthId=${monthId}`,
        method: 'GET',
        success: function (json) {
            if (!json.data) {
                console.warn("No data found.");
                return;
            }
            console.log("data", json.data)
            // Step 1: Reshape data by employee
            let grouped = {};
            let allDates = [];

            json.data.forEach(item => {
                let date = item.attendanceDate.split('T')[0];
                allDates.push(date);
                let daycount = item.dayCount || '0'; // Use daycount value

                if (!grouped[item.employeeId]) {
                    grouped[item.employeeId] = {
                        employeeName: item.employeeName,
                        jobCode: item.jobCode
                    };
                }

                grouped[item.employeeId][date] = daycount;
            });


            allDates = [...new Set(allDates)].sort(); // Unique and sorted

            // Step 2: Build columns
            const baseColumns = [
                { data: 'employeeName', title: 'Employee Name', className: 'text-left' },
                { data: 'jobCode', title: 'Job Code', className: 'text-center' }
            ];

            const dateColumns = allDates.map(date => {
                const dateObj = new Date(date);
                const formattedDate = dateObj.toLocaleDateString('en-GB', {
                    day: '2-digit',
                    month: 'short',
                    year: '2-digit'
                }).replace(/ /g, '-');

                return {
                    title: formattedDate,
                    data: date,
                    orderable: false,
                    className: 'text-center',
                    defaultContent: '0',
                    render: function (data) {
                        return data || '0';
                    }
                };
            });

            // Total Days column
            const totalDaysColumn = {
                data: 'totalDays',
                title: 'Total Days',
                className: 'text-center',
                orderable: false
            };

            const allColumns = baseColumns.concat(dateColumns, totalDaysColumn);

            // Step 3: Calculate totalDays
            Object.values(grouped).forEach(employee => {
                let total = 0;
                allDates.forEach(date => {
                    total += parseInt(employee[date] || '0');
                });
                employee.totalDays = total;
            });

            // Step 4: Render DataTable
            if ($.fn.DataTable.isDataTable('#dailyAttendencelist')) {
                _dataTable.destroy();
                $('#dailyAttendencelist').empty();
            }

            _dataTable = $('#dailyAttendencelist').DataTable({
                data: Object.values(grouped),
                columns: allColumns,
                pageLength: pageLength || 10,
                dom: '<"d-flex justify-content-between align-items-center"Bf>tip',
                buttons: [
                    {
                        extend: 'excelHtml5',
                        text: 'Excel',
                        title: null, // Prevents default title from being added
                        customize: function (xlsx) {
                            var sheet = xlsx.xl.worksheets['sheet1.xml'];
                            var downrows = 4; // Number of custom header rows to add
                            var clRow = $('row', sheet);

                            // Shift existing rows down
                            clRow.each(function () {
                                var attr = $(this).attr('r');
                                var ind = parseInt(attr);
                                ind = ind + downrows;
                                $(this).attr("r", ind);
                            });

                            // Update cell references
                            $('row c', sheet).each(function () {
                                var attr = $(this).attr('r');
                                var pre = attr.substring(0, 1);
                                var ind = parseInt(attr.substring(1));
                                ind = ind + downrows;
                                $(this).attr("r", pre + ind);
                            });

                            // Function to create a new row
                            function Addrow(index, data) {
                                var row = $('<row>').attr('r', index);
                                data.forEach(function (item) {
                                    var c = $('<c>').attr({ t: 'inlineStr', r: item.key + index });
                                    var is = $('<is>');
                                    var t = $('<t>').text(item.value);
                                    is.append(t);
                                    c.append(is);
                                    row.append(c);
                                });
                                return row;
                            }

                            // Define your custom header content
                            var r1 = Addrow(1, [{ key: 'A', value: 'Bangladesh Gas Fields School & College' }]);
                            var r2 = Addrow(2, [{ key: 'A', value: 'Birashar, Brahmanbaria.' }]);
                            var r3 = Addrow(3, [{ key: 'A', value: 'Monthly Report of Daily Worker' }]);
                            var r4 = Addrow(4, [{ key: 'A', value: 'Period: April-2025' }]);

                            // Insert the new rows into the sheetData
                            var sheetData = $('sheetData', sheet);
                            sheetData.prepend(r4);
                            sheetData.prepend(r3);
                            sheetData.prepend(r2);
                            sheetData.prepend(r1);
                        }
                    }
                ],
                scrollX: true,
                responsive: true,
                autoWidth: false
            });
        },
        error: function () {
            alert('Failed to load attendance data.');
        }
    });
}










function getMonthId() {
    let month = $('#months').val();
    let year = $('#years').val();

    return (parseInt(year) * 100) + parseInt(month);
}

function loadInitialData() {
    let pageLength = 10;
    // Call months API
    $.ajax({
        url: '/api/Others/GetMonths',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            let months = responseObject.data;
            let currentMonth = new Date().getMonth() + 1;
            $('#months, #f_months').empty();
            $.each(months, function (key, item) {
                let selected = currentMonth === parseInt(item.monthId) ? 'selected' : '';
                $('#months, #f_months').append(`<option value=${item.monthId} ${selected}>${item.monthName}</option>`);
            });
        },
        error: function () {
            alert('Failed to load months.');
        }
    });

    // Call years API
    $.ajax({
        url: '/api/Others/GetYears',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            let years = responseObject.data;
            let currentYear = new Date().getFullYear();
            $('#years, #f_years').empty();
            $.each(years, function (key, item) {
                let selected = item == currentYear ? 'selected' : '';
                $('#years, #f_years').append(`<option value=${item} ${selected}>${item}</option>`);
            });
        },
        error: function () {
            alert('Failed to load years.');
        }
    });

    $.ajax({
        url: '/api/Employees/GetEmployeeTypes',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            let employeeTypes = responseObject.data;
            const $employeeType = $('#EmployeeTypeId');
            const $errorField = $('#EmployeeTypeIdError');

            $employeeType.empty();
            $employeeType.append('<option value="">Select One</option>');

            $.each(employeeTypes, function (key, item) {
                // Append only item with id 1 or 2
                if (item.id == 1 || item.id == 2 || item.id == 3) {
                    $employeeType.append(`<option value="${item.id}">${item.employeeTypeName}</option>`);
                }
            });

            // Reset error field
            $errorField.text('');
        },
        error: function (xhr, status, error) {
            console.error("Error fetching employee types:", error);
        }
    });

}

