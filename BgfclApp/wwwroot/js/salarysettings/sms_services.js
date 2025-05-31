$(document).ready(function () {
    loadInitialData();

    $('#submit_button').click(function () {
        var month = $('#months').val();
        var year = $('#years').val();
        var employeeTypeId = $('#employeeTypeId').val();
        let monthId = getMonthId(month, year); // Convert the selected month and year into a unique monthId

        $.ajax({
            type: "POST",
            url: `/api/SalarySettings/SmsService?monthId=${monthId}&employeeTypeId=${employeeTypeId}`,
            async: false,
            dataType: 'json',
            success: function (data) {
                console.log("data",data)
                var tbody = $('#data-table-body');
                tbody.empty();

                if (data.data.length > 0) {
                    $('#table').show();

                    $.each(data.data, function (index, item) {
                        
                        var row = '<tr>' +
                            `<td><input type="checkbox" data-jobcode=${item.jobCode} /></td>` +
                            '<td>' + item.jobCode + '</td>' +
                            '<td>' + item.employeeName + '</td>' +
                            '<td>' + item.designationName + '</td>' +
                            '<td>' + item.departmentName + '</td>' +
                            '<td>' + item.mobileNumber + '</td>' +
                            '</tr>';
                        tbody.append(row);
                    });
                } else {
                    $('#table').hide();
                }
            },
            error: function (xhr, status, error) {
                console.error("An error occurred: " + error);
                alert("Failed to load data. Please try again.");
            }
        });
    });

    $("#select_all").click(function () {
        $('#data-table-body input:checkbox').not(this).prop('checked', this.checked);
    });

    $('#send_button').click(function () {
        var month = $('#months').val();
        var year = $('#years').val();
        var employeeTypeId = $('#employeeTypeId').val();
        let jobCodes = '';

        $("#data-table-body input[type=checkbox]:checked").each(function () {
            jobCodes += $(this).data('jobcode')+',';
        });

        jobCodes = jobCodes.replace(/,\s*$/, "");

        $.ajax({
            type: "POST",
            url: `/api/SalarySettings/SendSms?month=${month}&year=${year}&employeeTypeId=${employeeTypeId}&jobCodes=${jobCodes}`,
            async: false,
            dataType: 'json',
            success: function (responseObject) {
                if (responseObject.statusCode == 200) {
                    showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                }
                if (responseObject.statusCode == 500) {
                    showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                }
            },
            error: function (xhr, status, error) {
                console.error("An error occurred: " + error);
                alert("Failed to load data. Please try again.");
            }
        });
    });

    function getMonthId(month, year) {
        return (parseInt(year) * 100) + parseInt(month);
    }

    // Function to load initial data for months, years, and employee types
    function loadInitialData() {
        // Load months
        $.ajax({
            url: '/api/Others/GetMonths',
            type: 'GET',
            async: false,
            dataType: 'json',
            success: function (responseObject) {
                let months = responseObject.data;
                let currentMonth = new Date().getMonth() + 1;
                $('#months').empty();
                $.each(months, function (key, item) {
                    if (currentMonth == parseInt(item.monthId)) {
                        $('#months').append(`<option value=${item.monthId} selected>${item.monthName}</option>`);
                    } else {
                        $('#months').append(`<option value=${item.monthId}>${item.monthName}</option>`);
                    }
                });
            },
            error: function (responseObject) {
                console.error("Failed to load months.");
            }
        });

        // Load years
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
                    if (item == currentYear) {
                        $('#years').append(`<option value=${item} selected>${item}</option>`);
                    } else {
                        $('#years').append(`<option value=${item}>${item}</option>`);
                    }
                });
            },
            error: function (responseObject) {
                console.error("Failed to load years.");
            }
        });

        // Load employee types
        $.ajax({
            url: '/api/Employees/GetEmployeeTypes',
            type: 'GET',
            async: false,
            dataType: 'json',
            success: function (responseObject) {
                let employeeTypes = responseObject.data;
                $('#employeeTypeId').empty();
                $('#employeeTypeId').append('<option value="0">Select one</option>');
                $.each(employeeTypes, function (key, item) {
                    if (item.id == 1 || item.id == 2) {
                        $('#employeeTypeId').append(`<option value=${item.id}>${item.employeeTypeName}</option>`);
                    }
                });
            },
            error: function (responseObject) {
                console.error("Failed to load employee types.");
            }
        });
    }
});
