let _dataTable = undefined;
$(document).ready(function () {
    loadInitialData();

    $('#isExcel').on('change', function () {
        if ($(this).is(':checked')) {
            $('#generateButton')
                .text('Export To Excel')
                .css({
                    'background-color': 'yellow',
                    'color': 'black',
                    'font-size': '12px',
                    'padding': '4px 8px' // Adjusts button size
                });
        } else {
            $('#generateButton')
                .text('Report To PDF')
                .css({
                    'background-color': '',
                    'color': '',
                    'font-size': '12px',
                    'padding': '4px 8px'
                });
        }
    });


    //$('#months').on('change', function () {
    //    let monthId = getMonthId();
    //    $('#bonus_form').css('display', 'none');
    //    $.ajax({
    //        url: '/api/Bonus/GetBonusByMonthId?monthId=' + monthId,
    //        type: 'Get',
    //        async: false,
    //        dataType: 'json',
    //        success: function (responseObject) {
    //            let bonus = responseObject.data;
    //            $('#BonusId').empty();
    //            $('#BonusId').append(`<option value='0'>Select Bonus</option>`);
    //            $.each(bonus, function (key, item) {
    //                $('#BonusId').append(`<option value=${item.id}>${item.bonusTitle}</option>`);

    //            });
    //        },
    //        error: function (responseObject) {
    //        }
    //    });

    //});

    //$('#years').on('change', function () {
    //    let monthId = getMonthId();
    //    $('#bonus_form').css('display', 'none');
    //    $.ajax({
    //        url: '/api/Bonus/GetBonusByMonthId?monthId=' + monthId,
    //        type: 'Get',
    //        async: false,
    //        dataType: 'json',
    //        success: function (responseObject) {
    //            let bonus = responseObject.data;
    //            $('#BonusId').empty();
    //            $('#BonusId').append(`<option value='0'>Select Bonus</option>`);
    //            $.each(bonus, function (key, item) {
    //                $('#BonusId').append(`<option value=${item.id}>${item.bonusTitle}</option>`);

    //            });
    //        },
    //        error: function (responseObject) {
    //        }
    //    });
    //});

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
    _dataTable = $('#bonus_process_list_table').DataTable({
        pageLength: 4, // Default page length if none is passed
        order: [[1, 'desc'], [2, 'desc']], // Order by month (column 1) and year (column 2) in descending order
        ajax: {
            url: '/api/Bonus/GetPayableBonusList', // API endpoint for data
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
                data: 'month', orderable: false,
                className: 'text-center'
            },
            {
                // Year column
                data: 'year', orderable: false,
                className: 'text-center'
            },
            {
                // Employee Type column
                data: 'bonusTitle', orderable: false,
                className: 'text-left'
            },
            {
                // Status column
                data: 'status', orderable: false,
                className: 'text-left'
            }
        ],
        pageLength: 20,
        lengthMenu: [[10, 20, 50, 100, -1], [10, 20, 50, 100, 'All']],
        lengthChange: true,
        dom: '<"d-flex justify-content-between align-items-center mb-2"lfB>t<"d-flex justify-content-between align-items-center mt-2"ip>',
        buttons: ['excel', 'pdf', 'print'],
        autoWidth: false,
        language: {
            lengthMenu: 'Data per page _MENU_  '
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

function getBonusprocessList() {
    let schoolId = parseInt($('#school_filter').val()) || 0;
    let roleId = parseInt($('#role_filter').val()) || 0;
    let department = $('#department_filter').val()?.trim() || "";
    let designation = $('#designation_filter').val()?.trim() || "";
    let employeeTypeId = parseInt($('#employee_type_filter').val()) || 1;

    console.log({ schoolId, roleId, department, designation, employeeTypeId });

    // Determine API URL
    let apiUrl = (schoolId > 0 || roleId > 0 || department !== "" || designation !== "")
        ? '/api/Bonus/GetBonusProcessDataWithFilter'
        : '/api/Bonus/GetAllBonusProcessData';

    // Destroy existing DataTable (optional, or just reload it)


    //if ($.fn.DataTable.isDataTable("#process_bonus_list_table")) {
    //    $('#process_bonus_list_table').DataTable().ajax.url(apiUrl).load();
    //    return;
    //}


    // Initialize DataTable
    //_dataTable = $('#process_bonus_list_table').DataTable({
    //    order: [[1, 'desc'], [2, 'desc']],
    //    pageLength: 20,
    //    lengthMenu: [[10, 20, 50, 100, -1], [10, 20, 50, 100, 'All']],
    //    ajax: {
    //        url: apiUrl,
    //        type: 'GET',
    //        data: function (d) {
    //            d.employeeTypeId = employeeTypeId;
    //            d.school_filter = schoolId;
    //            d.role_filter = roleId;
    //            d.department_filter = department;
    //            d.designation_filter = designation;
    //        },
    //        dataSrc: function (json) {
    //            try {
    //                if (typeof json === 'string') {
    //                    json = JSON.parse(json);
    //                }
    //                return Array.isArray(json) ? json : json?.data ?? [];
    //            } catch (e) {
    //                alert('Invalid response from server. Please reload the page.');
    //                return [];
    //            }
    //        },
    //        error: function (jqXHR, textStatus, errorThrown) {
    //            console.error('AJAX Error:', textStatus, errorThrown);
    //            alert('Error loading bonus data:\n' + jqXHR.responseText);
    //        }
    //    },
    //    columns: [
    //        {
    //            data: null,
    //            className: 'text-center',
    //            render: function (data, type, row, meta) {
    //                return meta.row + meta.settings._iDisplayStart + 1;
    //            }
    //        },
    //        { data: 'jobCode', className: 'text-center', orderable: false, defaultContent: '' },
    //        { data: 'employeeName', className: 'text-left', orderable: false, defaultContent: '' },
    //        { data: 'designationName', className: 'text-left', orderable: false, defaultContent: '' },
    //        { data: 'departmentName', className: 'text-left', orderable: false, defaultContent: '' },
    //        { data: 'festivalBonus', className: 'text-right', orderable: false, render: data => data ? parseFloat(data).toLocaleString() : '-' },
    //        { data: 'incentiveBonus', className: 'text-right', orderable: false, render: data => data ? parseFloat(data).toLocaleString() : '-' },
    //        { data: 'honorariumBonus', className: 'text-right', orderable: false, render: data => data ? parseFloat(data).toLocaleString() : '-' },
    //        { data: 'scholarshipBonus', className: 'text-right', orderable: false, render: data => data ? parseFloat(data).toLocaleString() : '-' },
    //        { data: 'revStamp', className: 'text-right', orderable: false, render: data => data ? parseFloat(data).toLocaleString() : '-' },
    //        { data: 'deduction', className: 'text-right', orderable: false, render: data => data ? parseFloat(data).toLocaleString() : '-' }
    //    ],
    //    dom: '<"d-flex justify-content-between align-items-center mb-2"lfB>t<"d-flex justify-content-between align-items-center mt-2"ip>',
    //    buttons: ['copy', 'excel', 'pdf', 'print'],
    //    language: {
    //        lengthMenu: 'Data per page _MENU_'
    //    },
    //    responsive: true,
    //    autoWidth: false
    //});
}




// Call getBonusprocessList when filters are changed

//$('#school_filter, #role_filter, #department_filter, #designation_filter, #employee_type_filter').on('change', function () {
//    getBonusprocessList(); // Ensure this is the correct function name
//});



// Custom page length dropdown
$('#page_length_select').on('change', function () {
    let length = parseInt($(this).val(), 10);
    _dataTable.page.len(length).draw();  // Update the page length dynamically and redraw the table
});

function getMonthId() {
    let month = $('#months').val();
    let year = $('#years').val();

    return (parseInt(year) * 100) + parseInt(month);
}

function loadInitialData() {
    let pageLength = 10;
    getProcessList(pageLength)
   // getBonusprocessList();
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
        url: '/api/Bonus/GetBonus',
        type: 'GET',
        dataType: 'json',
        success: function (responseObject) {
            const bonuses = responseObject.data;
            const $bonusSelect = $('#BonusId');
            $bonusSelect.empty();
            $bonusSelect.append(`<option value="0">Select Bonus</option>`);
            $.each(bonuses, function (key, item) {
                $bonusSelect.append(`<option value="${item.id}">${item.bonusTitle}</option>`);
            });
        },
        error: function () {
            alert('Failed to load bonuses.');
        }
    });
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
            $('#EmployeeTypeId').empty();
            $('#EmployeeTypeId').append('<option value="0">Select One</option>');
            $.each(employeeTypes, function (key, item) {
                $('#EmployeeTypeId').append(`<option value="${item.id}">${item.employeeTypeName}</option>`);
            });
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
                $('#department_filter').append(`<option value=${item.departmentName}>${item.departmentName}</option>`);
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
                $('#designation_filter').append(`<option value=${item.designationName}>${item.designationName}</option>`);
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
            $('#employee_type_filter').append('<option value="0">All Types</option>');
            $.each(employeeTypes, function (key, item) {
                $('#employee_type_filter').append(`<option value=${item.id}>${item.employeeTypeName}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });
}

