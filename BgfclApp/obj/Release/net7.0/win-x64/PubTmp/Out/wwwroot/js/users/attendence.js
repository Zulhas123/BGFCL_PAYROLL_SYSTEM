let _dataTable = undefined;
var jss;
$(document).ready(function () {
    loadInitialData();
    getAttendanceMasterList();

    $('#Submit_btn').on('click', function (e) {
        console.log("Click")
        e.preventDefault(); 
        let months = $('#months').val();
        let years = $('#years').val();
        let startDate = $('#startDate').val();
        let endDate = $('#endDate').val();
        let isActive = $('#IsActive').is(":checked");
        let operationType = $('#operation_type').val();
        let monthId = getMonthId();
        let attendenceId = parseInt($('#AttendenceId').val()) || 0; 
        let dataObj = {
            id: attendenceId,
            months: months,
            years: years,
            startDate: startDate,
            endDate: endDate,
            monthId: monthId,
            isActive: true
        };

        let url = operationType === 'create' ? '/api/Users/CreateAttendenceMaster' : '/api/Users/UpdateAttendenceMaster';
        let method = operationType === 'create' ? 'POST' : 'PUT';

        // Make the AJAX request
        $.ajax({
            url: url,
            type: method,
            contentType: 'application/json',
            data: JSON.stringify(dataObj),
            success: function (responseObject) {
                // Clear error items
                $('.error-item').empty();

                if (responseObject.statusCode === 200 || responseObject.statusCode === 201) {
                    showToast('Success', responseObject.responseMessage, 'success');
                    getAttendanceMasterList();
                } else {
                    showToast('Error', responseObject.responseMessage, 'error');
                    getAttendanceMasterList();
                }
            },
            error: function (xhr) {
                let errorMessage = "An error occurred. Please try again.";
                if (xhr.responseJSON && xhr.responseJSON.responseMessage) {
                    errorMessage = xhr.responseJSON.responseMessage;
                }
                showToast('Error', errorMessage, 'error');
                console.error("Error response:", xhr.responseText);
                getAttendanceMasterList();
            }
        });
    });

    // Handle the "Cancel" button click event
    $('#cancel_button').on('click', function () {
        $('#attendenceformId')[0].reset(); // Reset the form
        $('#Submit_btn').text('Save'); 
    });
});
function getMonthId() {
    let month = $('#months').val();
    let year = $('#years').val();

    return (parseInt(year) * 100) + parseInt(month);
}
// Function to load initial data for months and years

function getAttendanceMasterList() {
    console.log("list");
    if (_dataTable) {
        _dataTable.destroy();
        _dataTable = null;
    }

    _dataTable = $('#Attendance_Master_list_table').DataTable({
        scrollX: true, // Enables horizontal scroll
        autoWidth: false,
        columnDefs: [
            { targets: 0, width: '5%' },   // Sl.
            { targets: 1, width: '10%' },  // Month ID
            { targets: 2, width: '12%' },  // Start Date
            { targets: 3, width: '12%' },  // End Date
            { targets: 4, width: '8%' },  // IsActive
            { targets: 5, width: '28%' }   // Action buttons
        ],
        ajax: {
            url: '/api/Users/GetAttendenceMaster',
            dataSrc: function (json) {
                console.log("API Response: ", json);
                return json && json.data ? (Array.isArray(json.data) ? json.data : [json.data]) : [];
            },
            error: function (xhr) {
                console.error("Error loading attendance:", xhr.responseText);
            }
        },
        columns: [
            {
                data: null,
                className: 'text-center align-middle',
                render: function (data, type, row, meta) {
                    if (!meta || !meta.settings) return '#';
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: 'monthId', className: 'text-center', orderable: false },
            {
                data: 'startDate',
                className: 'text-center',
                orderable: false,
                render: function (data) {
                    if (!data) return '';
                    const date = new Date(data);
                    const day = date.getDate();
                    const month = date.toLocaleString('en-US', { month: 'short' });
                    const year = date.getFullYear().toString().slice(-2);
                    return `${day}-${month}-${year}`;
                }
            },
            {
                data: 'endDate',
                className: 'text-center',
                orderable: false,
                render: function (data) {
                    if (!data) return '';
                    const date = new Date(data);
                    const day = date.getDate();
                    const month = date.toLocaleString('en-US', { month: 'short' });
                    const year = date.getFullYear().toString().slice(-2);
                    return `${day}-${month}-${year}`;
                }
            },
            {
                data: 'isActive',
                className: 'text-center',
                orderable: false,
                render: function (data) {
                    return data ? '<span class="text-success">Active</span>' : '<span class="text-danger">Inactive</span>';
                }
            },
            {
                data: 'id',
                className: 'text-center align-middle',
                orderable: false,
                render: function (data, type, row) {
                    return `
                        <button type="button" class="btn btn-primary btn-sm" onclick="onProcessClicked(${row.id})">
                            <i class="fas fa-play"></i> Process
                        </button> 
                        <button type="button" class="btn btn-info btn-sm" onclick="onEditClicked(${row.id})">
                            <i class="fas fa-edit"></i>
                        </button> 
                        <button type="button" class="btn btn-danger btn-sm" onclick="onRemoveClicked(${row.id})">
                            <i class="fas fa-trash"></i>
                        </button>
                        <button type="button" class="btn btn-primary btn-sm" onclick="onShowSpreadsheetClicked(${row.monthId}, ${row.id})">
                            <i class="fas fa-play"></i> Show
                        </button>
                    `;
                }

            }
        ],
        lengthMenu: [[10, 20, 50, 100, -1], [10, 20, 50, 100, 'All']],
        lengthChange: true,
        dom: '<"d-flex justify-content-between align-items-center mb-2"lfB>t<"d-flex justify-content-between align-items-center mt-2"ip>',
        buttons: ['copy', 'excel', 'pdf', 'print'],
        language: {
            lengthMenu: 'Data per page _MENU_  '
        },
        initComplete: function () {
            const api = this.api();

            // Remove existing filters if any
            $('#Attendance_Master_list_table thead th').find('.column-filter, br').remove();

            // Add column filters (except for the first and last columns)
            $('#Attendance_Master_list_table thead th').each(function (index) {
                const title = $(this).text();
                if (index !== 0 && title && title !== 'Action') {
                    $(this).append('<br><input type="text" class="form-control form-control-sm column-filter" placeholder=" " />');
                }
            });

            // Hook filter inputs to the columns
            api.columns().every(function () {
                const column = this;
                $('input.column-filter', column.header()).on('keyup change', function () {
                    if (column.search() !== this.value) {
                        column.search(this.value).draw(false);
                    }
                });
            });
        }
    });
}

function onEditClicked(AttendenceId) {
    AttendenceId = parseInt(AttendenceId, 10);
    if (isNaN(AttendenceId) || AttendenceId <= 0) {
        console.error("Invalid AttendenceId:", AttendenceId);
        showToast('Error', 'Invalid AttendenceId.', 'error');
        return;
    }

    $('#operation_type').val('edit');
    $('#Submit_btn').text('Update'); // optional: change Save to Update
    $('#AttendenceId').val(AttendenceId);

    $.ajax({
        url: `/api/Users/GetAttenceMasterById?id=${AttendenceId}`,
        type: 'GET',
        success: function (responseObject) {
            console.log("API Response for Edit:", responseObject);
            if (responseObject.statusCode === 200 && responseObject.data) {
                const data = responseObject.data;
                //$('#AttendenceId').val(data.attendenceId);

                let monthId = data.monthId.toString();
                let year = monthId.substring(0, 4);
                let month = parseInt(monthId.substring(4));
                $('#AttendenceId').val(data.id);
                $('#months').val(month);
                $('#years').val(year);
                $('#startDate').val(data.startDate?.substring(0, 10));
                $('#endDate').val(data.endDate?.substring(0, 10));
            } else {
                showToast('Error', responseObject.responseMessage || 'Attendance not found.', 'error');
            }
        },
        error: function () {
            showToast('Error', 'An error occurred while fetching attendance data.', 'error');
        }
    });
}

function onRemoveClicked(attendenceId) {
    $('#remove_modal').modal('show');
    $('#AttendenceId').val(attendenceId);
}

function onRemoveConfirmed() {
    let attendenceId = $('#AttendenceId').val();
    $('#remove_modal').modal('hide');

    $.ajax({
        url: `/api/Users/DeleteAttendenceMaster?id=${attendenceId}`,
        type: 'DELETE',
        success: function (response) {
            if (response.statusCode === 200) {
                showToast('Success', response.responseMessage, 'success');
                getAttendanceMasterList();
            } else {
                showToast('Error', response.responseMessage, 'error');
            }
        },
        error: function () {
            showToast('Error', 'An error occurred while deleting the role.', 'error');
        }
    });
}




function loadInitialData() {
    // Call years API to populate the year dropdown
    $.ajax({
        url: '/api/Others/GetYears',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            let years = responseObject.data;
            let currentYear = new Date().getFullYear();
            $('#years').empty();
            $.each(years, function (key, item) {
                let selected = item == currentYear ? 'selected' : '';
                $('#years').append(`<option value=${item} ${selected}>${item}</option>`);
            });
        },
        error: function () {
            alert('Failed to load years.');
        }
    });

    // Call months API to populate the month dropdown
    $.ajax({
        url: '/api/Others/GetMonths',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            let months = responseObject.data;
            let currentMonth = new Date().getMonth() + 1; // Get the current month (1-based index)
            $('#months').empty();
            $.each(months, function (key, item) {
                let selected = currentMonth === parseInt(item.monthId) ? 'selected' : '';
                $('#months').append(`<option value=${item.monthId} ${selected}>${item.monthName}</option>`);
            });
        },
        error: function () {
            alert('Failed to load months.');
        }
    });
}

// Function to show toast notifications (you can replace it with your own toast library)
function showToast(title, message, toastrType) {
    // Example: Implement your toast notifications logic here
    console.log(`${toastrType.toUpperCase()}: ${title} - ${message}`);
}

// Declare globally so it's accessible in onProcessClicked
let employees = [];

// Load employee data from API
$.ajax({
    url: '/api/Employees/GetEmployees?employeeTypeId=3',
    type: 'GET',
    async: false,
    dataType: 'json',
    success: function (responseObject) {
        console.log("employee", responseObject);
        employees = responseObject.data || [];
    },
    error: function (responseObject) {
        console.error("Error fetching employee data:", responseObject);
    }
});

// Process attendance on button click
function onProcessClicked(attendenceId) {
    if (!employees || employees.length === 0) {
        alert("No employee data available.");
        return;
    }

    if (!confirm("Are you sure you want to process attendance for this attendance ID?")) {
        return;
    }
    console.log("attendanceId:",attendenceId)
    // Step 1: Get startDate and endDate from the attendance master
    $.ajax({
        url: '/api/Users/GetAttenceMasterById?id=' + attendenceId,
        type: 'GET',
        success: function (response) {
            const attendanceMaster = response && response.data;

            if (!attendanceMaster || !attendanceMaster.startDate || !attendanceMaster.endDate) {
                alert("Start or End date not found for the selected attendance.");
                return;
            }

            const startDate = new Date(attendanceMaster.startDate.split('T')[0] + 'T00:00:00');
            startDate.setDate(startDate.getDate() + 1);
            const endDate = new Date(attendanceMaster.endDate.split('T')[0] + 'T00:00:00');
            endDate.setDate(endDate.getDate() + 1);

            const selectedData = employees.map(emp => {
                const attendanceRecords = [];

                for (let d = new Date(startDate); d <= endDate;) {
                    attendanceRecords.push({
                        AttendenceMasterId: attendenceId,
                        MonthId: attendanceMaster.monthId,
                        EmployeeId: emp.id,
                        EmployeeName: emp.employeeName,
                        JobCode: emp.jobCode,
                        AttendanceDate: new Date(d), // clone
                        DayCount: 1,
                        IsPresent: true
                    });

                    d.setDate(d.getDate() + 1);
                }

                return attendanceRecords;
            }).flat();


            console.log("selectedData", selectedData)
            // Step 3: Save attendance
            $.ajax({
                url: '/api/Users/SaveAttendance',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(selectedData),
                success: function (response) {
                    alert('Attendance processed and saved.');
                },
                error: function (xhr, status, error) {
                    console.error("Error processing attendance:", error);
                    alert('Failed to process attendance.');
                }
            });
        },
        error: function (xhr) {
            console.error("Error fetching attendance master:", xhr.responseText);
            alert('Failed to fetch attendance master data.');
        }
    });
}


function onShowClicked(monthId, attendenceMasterId) {
    console.log("monthId", monthId, "attendenceMasterId", attendenceMasterId);

    $.ajax({
        url: `/api/Users/GetAttendanceByMonthAndMaster?monthId=${monthId}&attendenceMasterId=${attendenceMasterId}`,
        type: 'GET',
        success: function (response) {
            if (!response || !response.data || response.data.length === 0) {
                $('#attendance-xml-view').html('<p>No attendance data available.</p>');
                return;
            }

            const records = response.data;
            const uniqueDates = [...new Set(records.map(r => new Date(r.attendanceDate).toISOString().split('T')[0]))];
            uniqueDates.sort();

            const attendanceMap = {};
            records.forEach(r => {
                const key = `${r.employeeId}_${new Date(r.attendanceDate).toISOString().split('T')[0]}`;
                attendanceMap[key] = r.dayCount ?? 1;
            });

            let xmlTable = `
                <div class="xml-scroll-wrapper">
                    <table class="table table-bordered table-striped xml-table">
                        <thead>
                            <tr>
                                <th>Employee Name</th>
                                <th>Job Code</th>`;

            uniqueDates.forEach(date => {
                const d = new Date(date);
                const formatted = `${d.getDate()}-${d.toLocaleString('default', { month: 'short' })}-${d.getFullYear().toString().slice(-2)}`;
                xmlTable += `<th class="date-col">${formatted}</th>`;
            });

            xmlTable += `<th class="total-col">Day Count</th></tr></thead><tbody>`;

            employees.forEach(emp => {
                let totalDays = 0;
                xmlTable += `<tr>
                    <td>${emp.employeeName}</td>
                    <td>${emp.jobCode}</td>`;

                uniqueDates.forEach(date => {
                    const key = `${emp.id}_${date}`;
                    const value = attendanceMap[key] ?? 1;
                    totalDays += value;

                    xmlTable += `<td>
                        <input type="number" class="form-control form-control-sm day-count-input"
                            data-employee-id="${emp.id}" data-date="${date}"
                            data-job-code="${emp.jobCode}" data-employee-name="${emp.employeeName}"
                            value="${value}" min="0" />
                    </td>`;
                });

                xmlTable += `<td class="day-count-total">${totalDays}</td></tr>`;
            });

            xmlTable += `</tbody></table></div>
                <button class="btn btn-success btn-sm mt-2" onclick="saveDayCountUpdates(${monthId}, ${attendenceMasterId})">Save Changes</button>`;

            $('#attendance-xml-view').html(xmlTable);

            // Auto-update total
            $('.day-count-input').on('input', function () {
                const row = $(this).closest('tr');
                let total = 0;
                row.find('.day-count-input').each(function () {
                    const val = parseFloat($(this).val());
                    if (!isNaN(val)) total += val;
                });
                row.find('.day-count-total').text(total);
            });
        },
        error: function (xhr) {
            console.error("Failed to load attendance data:", xhr.responseText);
        }
    });
}

function saveDayCountUpdates(monthId, attendenceMasterId) {
    const records = [];

    $('.day-count-input').each(function () {
        const input = $(this);
        const dayCount = parseFloat(input.val());

        if (!isNaN(dayCount)) {
            records.push({
                attendenceMasterId: attendenceMasterId,
                monthId: monthId,
                employeeId: input.data('employee-id'),
                jobCode: input.data('job-code'),
                employeeName: input.data('employee-name'),
                attendanceDate: input.data('date'),
                dayCount: dayCount,
                isPresent: dayCount > 0
            });
        }
    });

    if (records.length === 0) {
        showToast('Warning', 'No valid attendance data to save.', 'warning');
        return;
    }

    // Step 1: Save attendance records
    $.ajax({
        url: '/api/Users/UpdateAttendance',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(records),
        success: function (response) {
            showToast('Success', response.message || 'Attendance saved successfully.', 'success');

            // Step 2: Then update attendance master
            const months = $('#months').val();
            const years = $('#years').val();
            const startDate = $('#startDate').val();
            const endDate = $('#endDate').val();
            const isActive = $('#IsActive').is(":checked");
            const operationType = $('#operation_type').val();
            const monthId = getMonthId();
            const attendenceId = parseInt($('#AttendenceId').val()) || 0;

            const dataObj = {
                id: attendenceId,
                months: months,
                years: years,
                startDate: startDate,
                endDate: endDate,
                monthId: monthId,
                isActive: isActive,
                operationType: operationType
            };

            $.ajax({
                url: '/api/Users/UpdateAttendenceMaster',
                type: attendenceId > 0 ? 'PUT' : 'POST', // fallback for `method`
                contentType: 'application/json',
                data: JSON.stringify(dataObj),
                success: function (responseObject) {
                    $('.error-item').empty();

                    if (responseObject.statusCode === 200 || responseObject.statusCode === 201) {
                        showToast('Success', responseObject.responseMessage, 'success');
                    } else {
                        showToast('Error', responseObject.responseMessage, 'error');
                    }

                    getAttendanceMasterList(); // refresh list in both cases
                },
                error: function (xhr) {
                    const errorMessage = xhr.responseJSON?.responseMessage || "An error occurred. Please try again.";
                    showToast('Error', errorMessage, 'error');
                    console.error("Error response:", xhr.responseText);
                    getAttendanceMasterList();
                }
            });
        },
        error: function (xhr) {
            const errorMessage = xhr.responseJSON?.message || "An error occurred while saving attendance.";
            showToast('Error', errorMessage, 'error');
            console.error("Error response:", xhr.responseText);
        }
    });
}
// Spread sheet 
function onShowSpreadsheetClicked(monthId, attendenceMasterId) {
    if (typeof jss !== 'undefined') {
        jss.destroy();
        $('#spreadsheet').empty();
    }

    console.log("Triggered onShowSpreadsheetClicked");
    console.log("monthId:", monthId, "attendenceMasterId:", attendenceMasterId);

    $.ajax({
        url: '/api/Users/GetAttenceMasterById?id=' + attendenceMasterId,
        type: 'GET',
        success: function (response) {
            const attendanceMaster = response?.data || response;

            if (!attendanceMaster?.startDate || !attendanceMaster?.endDate) {
                alert("Start or End date not found for the selected attendance.");
                return;
            }

            const startDate = new Date(attendanceMaster.startDate);
            startDate.setDate(startDate.getDate() + 1);
            const endDate = new Date(attendanceMaster.endDate);
            endDate.setDate(endDate.getDate() + 1);

            const uniqueDates = [];
            let tempDate = new Date(startDate);
            while (tempDate <= endDate) {
                uniqueDates.push(tempDate.toISOString().split('T')[0]);
                tempDate.setDate(tempDate.getDate() + 1);
            }

            $.ajax({
                url: `/api/Users/GetAttendanceByMonthAndMaster?monthId=${monthId}&attendenceMasterId=${attendenceMasterId}`,
                type: 'GET',
                success: function (response) {
                    if (!response?.data?.length) {
                        showToast('Info', 'No attendance data found.', 'info');
                        return;
                    }

                    const records = response.data;
                    const attendanceMap = {};
                    const employeeMap = {};

                    records.forEach(r => {
                        const dateKey = new Date(r.attendanceDate).toISOString().split('T')[0];
                        const key = `${r.employeeId}_${dateKey}`;
                        attendanceMap[key] = r.dayCount ?? 1;

                        if (!employeeMap[r.employeeId]) {
                            employeeMap[r.employeeId] = {
                                employeeId: r.employeeId,
                                employeeName: r.employeeName,
                                jobCode: r.jobCode
                            };
                        }
                    });

                    const columns = [
                        { type: 'hidden', title: 'Employee ID', name: 'employeeId' },
                        { type: 'text', title: 'Employee Name', name: 'employeeName', width: 150, readOnly: true },
                        { type: 'text', title: 'Job Code', name: 'jobCode', width: 100, readOnly: true }
                    ];

                    uniqueDates.forEach(date => {
                        const d = new Date(date);
                        const formatted = d.toLocaleDateString('en-GB', {
                            day: '2-digit',
                            month: 'short',
                            year: '2-digit'
                        }).replace(/ /g, '-');

                        columns.push({
                            type: 'numeric',
                            title: formatted,
                            name: date,
                            width: 60
                        });
                    });

                    columns.push({
                        type: 'numeric',
                        title: 'Total Days',
                        name: 'totalDays',
                        width: 80,
                        readOnly: true
                    });

                    const data = [];
                    Object.values(employeeMap).forEach(emp => {
                        const row = [emp.employeeId, emp.employeeName, emp.jobCode];
                        let total = 0;

                        uniqueDates.forEach(date => {
                            const key = `${emp.employeeId}_${date}`;
                            const day = attendanceMap[key] ?? 1;
                            row.push(day);
                            total += day;
                        });

                        row.push(total);
                        data.push(row);
                    });

                    window.updatedRowsMap = {};

                    jss = jspreadsheet(document.getElementById('spreadsheet'), {
                        data: data,
                        columns: columns,
                        freezeColumns: 3,
                        tableOverflow: true,
                        tableWidth: window.innerWidth - 100 + "px",
                        columnSorting: false,
                        contextMenu: () => [],
                        onchange: function (instance, cell, x, y, value) {
                            console.log("⚠️ Cell changed at X:", x, "Y:", y, "Value:", value);

                            const employeeId = jss.getValueFromCoords(0, y);
                            const employeeName = jss.getValueFromCoords(1, y);
                            const jobCode = jss.getValueFromCoords(2, y);

                            let total = 0;
                            const updates = [];

                            const dateStartColIndex = 3;
                            const totalColIndex = 3 + uniqueDates.length;

                            uniqueDates.forEach((date, index) => {
                                const colIndex = dateStartColIndex + index;
                                let dayCount;

                                if (colIndex === x) {
                                    console.log("📌 Editing date column:", date, "New Value:", value);
                                    dayCount = parseFloat(value);
                                } else {
                                    dayCount = parseFloat(jss.getValueFromCoords(colIndex, y));
                                }

                                if (!isNaN(dayCount)) {
                                    total += dayCount;

                                    updates.push({
                                        attendenceMasterId: attendenceMasterId,
                                        monthId: monthId,
                                        employeeId,
                                        jobCode,
                                        employeeName,
                                        attendanceDate: date,
                                        dayCount,
                                        isPresent: dayCount > 0
                                    });
                                }
                            });

                            jss.setValueFromCoords(totalColIndex, y, total);

                            window.updatedRowsMap[employeeId] = updates;
                        }
                    });

                    $('#update_button')
                        .show()
                        .off('click')
                        .on('click', function () {
                            saveSpreadsheetUpdates(monthId, attendenceMasterId);
                        });
                },
                error: function (xhr) {
                    showToast('Error', xhr.responseText || 'Error loading attendance data.', 'error');
                }
            });
        },
        error: function (xhr) {
            showToast('Error', xhr.responseText || 'Error loading attendance master data.', 'error');
        }
    });
}





let updatedRowsMap = {};

function saveSpreadsheetUpdates(monthId, attendanceMasterId) {
    window.updatedRowsMap = window.updatedRowsMap || {};
    let finalUpdates = [];
    Object.values(window.updatedRowsMap).forEach(rows => {
        finalUpdates = finalUpdates.concat(rows);
    });

    console.log("Final updates to save:", finalUpdates);

    if (finalUpdates.length === 0) {
        showToast('Warning', 'No changes to save.', 'warning');
        return;
    }

    $.ajax({
        url: '/api/Users/UpdateAttendance',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(finalUpdates),
        success: function (response) {
            showToast('Success', response.message || 'Attendance saved.', 'success');
            // Clear updates before reload to avoid stale data
            window.updatedRowsMap = {};
            // Reload sheet data
            onShowSpreadsheetClicked(monthId, attendanceMasterId);
            $('#update_button').hide();
        },
        error: function (xhr) {
            showToast('Error', xhr.responseJSON?.message || 'Error saving attendance.', 'error');
        }
    });
}





