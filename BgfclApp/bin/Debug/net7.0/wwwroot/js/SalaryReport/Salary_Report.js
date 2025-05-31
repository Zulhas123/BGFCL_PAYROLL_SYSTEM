let _dataTable = undefined;
$(document).ready(function () {
    $('#jobCode').select2({
        placeholder: "Select job codes",
        allowClear: true
    });
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
    setTimeout(function () {
        var alertBox = document.getElementById("processAlert");
        if (alertBox) {
            alertBox.classList.remove("show");
            alertBox.classList.add("fade");
            setTimeout(() => {
                alertBox.style.display = 'none';
            }, 500); // Allow Bootstrap's fade animation
        }
    }, 5000);
    // Form submit validation
    $('#salarycontId').on('submit', function (e) {
        const employeeTypeValue = $('#EmployeeTypeId').val();
        const $errorField = $('#EmployeeTypeIdError');

        $errorField.text('');

        if (!employeeTypeValue) {
            e.preventDefault();
            $errorField.text('Please select an employee type.');
        }
    });

    // Real-time error clearing when a valid selection is made
    $('#EmployeeTypeId').on('change', function () {
        const employeeTypeValue = $(this).val();
        const $errorField = $('#EmployeeTypeIdError');

        if (employeeTypeValue) {
            $errorField.text('');
        }
    });
  
});


function getMonthName(monthNumber) {
    const monthNames = [
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];
    return monthNames[parseInt(monthNumber, 10) - 1]; // Convert "01" -> 0 index
}





function getProcessList(pageLength) {
    // Get current month and year
    let currentDate = new Date();
    let currentMonth = currentDate.getMonth() + 1;  // Month is 0-based, so add 1
    let currentYear = currentDate.getFullYear();

    // Check if the DataTable instance already exists, then destroy it
    if (typeof _dataTable !== 'undefined' && $.fn.DataTable.isDataTable('#salary_process_list_table')) {
        _dataTable.destroy();
    }

    // Initialize the DataTable
    _dataTable = $('#salary_process_list_table').DataTable({
        pageLength: 4, // Default page length if none is passed
        order: [[1, 'desc'], [2, 'desc']], // Order by month (column 1) and year (column 2) in descending order
        ajax: {
            url: '/api/SalarySettings/GetSalaryProcess', // API endpoint for data
            dataSrc: 'data', // Path to data in the response object
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error fetching data:', errorThrown);
                alert('Failed to load salary process data. Please try again later.');
            }
        },
        columns: [
            {
                // Serial number column
                data: null,
                className: 'text-center',
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1; // Serial number logic
                }
            },
            {
                // Month column
                data: 'month',
                orderable: false,
                className: 'text-center'
            },
            {
                // Year column
                data: 'year',
                orderable:false,
                className: 'text-center'
            },
            {
                // Employee Type column
                data: 'employeeType',
                orderable: false,
                className: 'text-center'
            },
            {
                // Status column with increased width
                data: 'status',
                orderable: false,
                className: 'text-left',
                width: '200px' // 👈 You can adjust this value as needed
            }
        ],
        dom: '<"d-flex justify-content-between align-items-center"Bf>tip', // Custom DOM layout
        buttons: [
            'copy', 'excel', 'pdf', 'print' // Export options
        ],
        language: {
            emptyTable: 'No salary process data available', // Custom message for empty table
            loadingRecords: 'Loading...', // Loading message
            zeroRecords: 'No matching records found' // Message for no matches
        },
        responsive: true, // Make the table responsive
        autoWidth: false, // Disable auto width adjustment

        initComplete: function () {
            let api = this.api();

            // Add column filters without replacing headers
            $('#salary_process_list_table thead th').each(function (index) {
                let title = $(this).text();
                if (index !== 0 && title && title !== 'status') {
                    $(this).append(`<br><input type="text" class="form-control form-control-sm column-filter" placeholder=" " />`);
                }
            });

            // Apply column filtering
            api.columns().every(function () {
                let that = this;
                $('input', this.header()).on('keyup change', function () {
                    if (that.search() !== this.value) {
                        that.search(this.value).draw();
                    }
                });
            });
        }
    });
}

function getSalaryReportList(pageLength) {
    if ($.fn.DataTable.isDataTable('#salary_report_list_table')) {
        _dataTable.destroy();
        $('#salary_report_list_table thead tr th input.column-filter').remove(); // Remove existing filters
    }

    // Get filter values properly
    let schoolId = $('#school_filter').val() || 0;
    let roleId = $('#role_filter').val() || 0;
    let departmentId = $('#department_filter').val() || 0;
    let designationId = $('#designation_filter').val() || 0;
    let employeeTypeId = $('#employee_type_filter').val() || 1; // Default to Officers

    console.log("typeid", employeeTypeId);

    // Determine API URL based on employeeTypeId
    let apiUrl = '';

    if (employeeTypeId == 1) {
        apiUrl = (schoolId != 0 || roleId != 0 || departmentId != 0 || designationId != 0)
            ? '/api/SalarySettings/GetreportOfficerWithFilter'
            : '/api/SalarySettings/GetSalaryReportOfficerData';
    } else if (employeeTypeId == 2) {
        apiUrl = (schoolId != 0 || roleId != 0 || departmentId != 0 || designationId != 0)
            ? '/api/SalarySettings/GetreportContractWithFilter'
            : '/api/SalarySettings/GetSalaryReportContractData';
    } else if (employeeTypeId == 3) {
        apiUrl = (schoolId != 0 || roleId != 0 || departmentId != 0 || designationId != 0)
            ? '/api/SalarySettings/GetreportGuestWithFilter'
            : '/api/SalarySettings/GetSalaryReportGuestData';
    } else {
        alert("Invalid Employee Type selected.");
        return;
    }

    // Initialize the DataTable
    _dataTable = $('#salary_report_list_table').DataTable({
        pageLength: pageLength || 10,
        order: [[1, 'desc']], // Order by MonthID (column 1) in descending order
        destroy: true, // Ensure re-initialization
        ajax: {
            url: apiUrl,
            type: 'GET',
            data: function (d) {
                d.employeeTypeId = employeeTypeId;
                d.school_filter = schoolId;
                d.role_filter = roleId;
                d.department_filter = departmentId;
                d.designation_filter = designationId;
            },
            dataSrc: 'data',
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('AJAX Error:', textStatus, errorThrown);
                console.error('Response:', jqXHR.responseText);
                alert(`Failed to load salary report data: ${jqXHR.status} - ${jqXHR.statusText}`);
            }

        },
        columns: [
            {
                data: null,
                className: 'text-center',
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1; // Serial number
                }
            },
            { data: 'monthId', className: 'text-center',orderable: false },
            { data: 'jobCode', className: 'text-center', orderable: false },
            { data: 'employeeName', className: 'text-left', orderable: false },
            { data: 'designationName', className: 'text-left', orderable: false, className: 'text-center' },
            { data: 'departmentName', className: 'text-left', orderable: false ,className: 'text-center' },
            {
                data: 'basicSalary',
                orderable: false,
                className: 'text-right',
                render: function (data) {
                    return data ? parseFloat(data).toLocaleString() : '-';
                }
            },
            {
                data: 'grossPay', orderable: false,
                className: 'text-right',
                render: function (data) {
                    return data ? parseFloat(data).toLocaleString() : '-';
                }
            },
            {
                data: 'totalDeduction', orderable: false,
                className: 'text-right',
                render: function (data) {
                    return data ? parseFloat(data).toLocaleString() : '-';
                }
            },
            {
                data: 'netPay', orderable: false,
                className: 'text-right',
                render: function (data) {
                    return data ? parseFloat(data).toLocaleString() : '-';
                }
            }
        ],
        pageLength: 20,
        lengthMenu: [[10, 20, 50, 100, -1], [10, 20, 50, 100, 'All']],
        lengthChange: true,
        dom: '<"d-flex justify-content-between align-items-center mb-2"lfB>t<"d-flex justify-content-between align-items-center mt-2"ip>',
        buttons: ['copy', 'excel', 'pdf', 'print'],
        autoWidth: false,
        language: {
            lengthMenu: 'Data per page _MENU_  '
        },
        responsive: true,
        autoWidth: false,

        initComplete: function () {
            let api = this.api();

            // **Fix: Ensure only one set of filter inputs is added**
            if ($('#salary_report_list_table thead tr th input.column-filter').length === 0) {
                $('#salary_report_list_table thead th').each(function (index) {
                    let title = $(this).text().trim();
                    if (title) {
                        $(this).append(`<input type="text" class="form-control form-control-sm column-filter" placeholder=" " />`);
                    }
                });

                // Apply column filtering
                api.columns().every(function () {
                    let that = this;
                    $('input.column-filter', this.header()).on('keyup change', function () {
                        if (that.search() !== this.value) {
                            that.search(this.value).draw();
                        }
                    });
                });
            }
        }
    });
}


// Call getSalaryReportList when filters are changed
$('#school_filter, #role_filter, #department_filter, #designation_filter, #employee_type_filter').on('change', function () {
    getSalaryReportList();
});


function getMonthId() {
    let month = $('#months').val();
    let year = $('#years').val();

    return (parseInt(year) * 100) + parseInt(month);
}

function loadInitialData() {
    let pageLength = 10;
    getProcessList(pageLength)
    getSalaryReportList(pageLength)
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

    $.ajax({
        url: '/api/Departments/GetDepartments',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            departments = responseObject.data;
            $('#department_filter').empty();
            $('#department_filter').append('<option value="0">All Department</option>');
            $.each(departments, function (key, item) {
                $('#department_filter').append(`<option value=${item.id}>${item.departmentName}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });
    $.ajax({
        url: '/api/Designations/GetDesignations',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            designations = responseObject.data;
            $('#designation_filter').empty();
            $('#designation_filter').append('<option value="0">All Designation</option>');
            $.each(designations, function (key, item) {
                $('#designation_filter').append(`<option value=${item.id}>${item.designationName}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });
    $.ajax({
        url: '/api/Roles/GetRole',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            roles = responseObject.data;
            $('#role_filter').empty();
            $('#role_filter').append('<option value="0">All Roles</option>');
            $.each(roles, function (key, item) {
                $('#role_filter').append(`<option value=${item.id}>${item.title}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });
    $.ajax({
        url: '/api/Schools/GetSchools',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            roles = responseObject.data;
            $('#school_filter').empty();
            $('#school_filter').append('<option value="0">All School</option>');
            $.each(roles, function (key, item) {
                $('#school_filter').append(`<option value=${item.id}>${item.title}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });

    $.ajax({
        url: '/api/Employees/GetEmployeeTypes',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            employeeTypes = responseObject.data;
            $('#employee_type_filter').empty();
            $('#employee_type_filter').append('<option value="">All Types</option>');
            $.each(employeeTypes, function (key, item) {
                $('#employee_type_filter').append(`<option value=${item.id}>${item.employeeTypeName}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });
    
}

