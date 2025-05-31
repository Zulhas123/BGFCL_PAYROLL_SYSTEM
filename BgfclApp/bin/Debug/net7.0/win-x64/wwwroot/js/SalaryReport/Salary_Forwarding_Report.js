let _dataTable = undefined;
$(document).ready(function () {
    loadInitialData();
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
});

/**
 * Returns the selected MonthID in format YYYYMM (e.g., "202501").
 */
function getMonthId() {
    let year = $('#Year').val();
    let month = $('#Month').val().padStart(2, '0'); // Ensure two digits
    return String(year) + String(month);
}

/**
 * Converts a numeric month (e.g., "01") to a month name (e.g., "January").
 */
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
        pageLength: 10, // Default page length if none is passed
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
                className: 'text-center'
            },
            {
                // Year column
                data: 'year',
                className: 'text-center'
            },
            {
                // Employee Type column
                data: 'employeeType',
                className: 'text-center'
            },
            {
                // Status column
                data: 'status',
                className: 'text-left',
                width: '200px'
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
                let title = $(this).text().trim();
                if (title && title !== 'Action') {
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


function getMonthId() {
    let month = $('#months').val();
    let year = $('#years').val();

    return (parseInt(year) * 100) + parseInt(month);
}

function loadInitialData() {
    let pageLength = 10;
    getProcessList(pageLength)

    $.ajax({
        url: '/api/Banks/GetAccounts',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            let accounts = responseObject.data;
            $('#Accounts').empty().append('<option value="">-- Select Account --</option>');
            $.each(accounts, function (key, item) {
                $('#Accounts').append(`<option value="${item.accountNumber}">${item.accountNumber}</option>`);
            });
        },
        error: function () {
            alert('Failed to load accounts.');
        }
    });


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

    // Load Employee Types
    $.ajax({
        url: '/api/Employees/GetEmployeeTypes',
        type: 'GET',
        dataType: 'json',
        success: function (responseObject) {
            const employeeTypes = responseObject.data;
            const $employeeType = $('#EmployeeTypeId');
            const $errorField = $('#EmployeeTypeIdError');

            $employeeType.empty();
            $employeeType.append('<option value="">Select One</option>');

            $.each(employeeTypes, function (key, item) {
                if (item.id == 1 || item.id == 2 || item.id == 3)  {
                    $employeeType.append(`<option value="${item.id}">${item.employeeTypeName}</option>`);
                }
            });

            $errorField.text('');
        },
        error: function (xhr, status, error) {
            console.error("Error fetching employee types:", error);
        }
    });

    // Validate before form submit
    $('#salaryForwardingFormId').on('submit', function (e) {
        const $employeeType = $('#EmployeeTypeId');
        const $errorField = $('#EmployeeTypeIdError');
        const employeeTypeValue = $employeeType.val();

        let isValid = true;
        $errorField.text('');

        if (!employeeTypeValue) {
            isValid = false;
            $errorField.text('Please select an employee type.');
        }

        if (!isValid) {
            e.preventDefault(); // Block form submission
        }
    });

    // Hide error on selection change
    $('#EmployeeTypeId').on('change', function () {
        const value = $(this).val();
        if (value) {
            $('#EmployeeTypeIdError').text('');
        }
    });
}

