let _dataTable = undefined;
$(document).ready(function () {
    loadInitialData();
    $('#EmployeeTypeId').on('change', function () {
        const selectedText = $("#EmployeeTypeId option:selected").text().toLowerCase();

        if (selectedText.includes('permanent')) {
            $('#bonusValueLabel').text('% of Basic Salary');
            $('#bonusValueInput').attr('placeholder', 'Enter percentage');
            $('#bonusValueGroup').show();
        } else if (selectedText.includes('contract')) {
            $('#bonusValueLabel').text('Fixed Amount');
            $('#bonusValueInput').attr('placeholder', 'Enter fixed amount');
            $('#bonusValueGroup').show();
        } else {
            $('#bonusValueGroup').hide();
            $('#bonusValueInput').val('');
        }
    });
    $('#cancel_button').on('click', () => {
        $('#bonusForm')[0].reset(); // ✅ reset the form
        $('#bonusModalLabel').text('');
        $('#bonusModal').modal('hide');
    });

});
$('#add_button').on('click', function () {
    $('#bonusModal').modal('show');
    $('#operation_type').val('create');
    $('#submit_button').html('Create');
    $('#bonusModalLabel').html('Process Bonus');
});

function getMonthId() {
    let month = $('#months').val();
    let year = $('#years').val();

    return (parseInt(year) * 100) + parseInt(month);
}
$('#process_btn').on('click', () => {
    $('.error-item').text('');

    const monthId = getMonthId();
    console.log("MonthId:", monthId);

    const dataObj = {
        SchoolId: +$('#schoolId').val(),
        GuestPkId: +$('#guestPkId').val(),
        RoleId: +$('#roleId').val(),
        UserId: +$('#userId').val(),
        MonthId: monthId,
        Month: +$('#months').val(),
        Year: +$('#years').val(),
        BonusId: +$('#BonusId').val(),
        EmployeeTypeId: +$('#EmployeeTypeId').val(),
        FestivalBonus: $('#FestivalBonus').val() ? +$('#FestivalBonus').val() : null
    };

    let hasError = false;
    if (!dataObj.BonusId) { $('#BonusIdError').text('Required'); hasError = true; }
    if (!dataObj.EmployeeTypeId) { $('#EmployeeTypeIdError').text('Required'); hasError = true; }
    if (hasError) return;

    $.ajax({
        url: '/api/Bonus/ProcessBonusData',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(dataObj),
        success: resp => {
            const type = resp.statusCode === 201 ? 'success' : 'error';
            $('#bonusModal').modal('hide');
            showToast(type === 'success' ? 'Success' : 'Error', resp.responseMessage, type);
            getBonusprocessList();
        },
        error: () => showToast('Error', 'Failed to process bonus data.', 'error')
    });
});


function getBonusprocessList() {
    let schoolId = parseInt($('#school_filter').val()) || 0;
    let roleId = parseInt($('#role_filter').val()) || 0;

    // Keep department and designation as strings
    let department = $('#department_filter').val()?.trim();
    let designation = $('#designation_filter').val()?.trim();

    let employeeTypeId = parseInt($('#employee_type_filter').val()) || 1;

    console.log("employeeTypeId:", employeeTypeId);

    // Clean up filter values
    if (department === "0" || department === null || department === "") department = "";
    if (designation === "0" || designation === null || designation === "") designation = "";

    // Determine API URL
    let apiUrl = (schoolId > 0 || roleId > 0 || department !== "" || designation !== "")
        ? '/api/Bonus/GetBonusProcessDataWithFilter'
        : '/api/Bonus/GetAllBonusProcessData';

    console.log("API URL:", apiUrl);

    // Destroy existing DataTable if present
    if ($.fn.DataTable.isDataTable("#process_bonus_list_table")) {
        $('#process_bonus_list_table').DataTable().destroy();
    }
    $('#process_bonus_list_table tbody').empty();

    // Initialize DataTable
    _dataTable = $('#process_bonus_list_table').DataTable({
        order: [[1, 'desc'], [2, 'desc']],
        pageLength: 20,
        lengthMenu: [[10, 20, 50, 100, -1], [10, 20, 50, 100, 'All']],
        ajax: {
            url: apiUrl,
            type: 'GET',
            dataType: 'json',
            data: function (d) {
                d.employeeTypeId = employeeTypeId;
                d.school_filter = schoolId;
                d.role_filter = roleId;
                d.department_filter = department;
                d.designation_filter = designation;
            },
            dataSrc: function (json) {
                if (typeof json === 'string') {
                    try {
                        json = JSON.parse(json);
                    } catch (e) {
                        console.error('Could not parse JSON:', json);
                        return [];
                    }
                }

                // Log for debugging
                console.log("Data received:", json.data);

                if (!json || !Array.isArray(json.data)) {
                    console.warn('Invalid or empty data:', json.data);
                    return [];
                }

                // Ensure data doesn't contain empty objects
                return json.data.filter(item => Object.keys(item).length > 0);
            },
            error: function (xhr, textStatus, errorThrown) {
                console.error('DataTables AJAX error:', textStatus, errorThrown);
                console.error('Response text:', xhr.responseText);
            }
        },
        columns: [
            {
                data: null, className: 'text-center', render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                data: 'jobCode', className: 'text-center', orderable: false, render: function (data) {
                    return data || '';
                }
            },
            {
                data: 'employeeName', className: 'text-left', orderable: false, render: function (data) {
                    return data || '';
                }
            },
            {
                data: 'designationName', className: 'text-left', orderable: false, render: function (data) {
                    return data || '';
                }
            },
            {
                data: 'departmentName', className: 'text-left', orderable: false, render: function (data) {
                    return data || '';
                }
            },
            {
                data: 'festivalBonus', className: 'text-right', orderable: false, render: function (data) {
                    return data ? parseFloat(data).toLocaleString() : '-';
                }
            },
            {
                data: 'revStamp', className: 'text-right', orderable: false, render: function (data) {
                    return data ? parseFloat(data).toLocaleString() : '-';
                }
            },
            {
                data: 'deduction', className: 'text-right', orderable: false, render: function (data) {
                    return data ? parseFloat(data).toLocaleString() : '-';
                }
            }
        ],
        lengthChange: true,
        dom: '<"d-flex justify-content-between align-items-center mb-2"lfB>t<"d-flex justify-content-between align-items-center mt-2"ip>',
        buttons: ['copy', 'excel', 'pdf', 'print'],
        language: {
            lengthMenu: 'Data per page <select class="form-control form-select form-select-sm">' +
                '<option value="10">10</option>' +
                '<option value="20" selected>20</option>' +
                '<option value="50">50</option>' +
                '<option value="100">100</option>' +
                '<option value="-1">All</option>' +
                '</select>',
            emptyTable: 'No salary process data available',
            loadingRecords: 'Loading...',
            zeroRecords: 'No matching records found'
        },
        responsive: true,
        autoWidth: false
    });
}




// Call getBonusprocessList when filters are changed
$('#school_filter, #role_filter, #department_filter, #designation_filter, #employee_type_filter').on('change', function () {
    getBonusprocessList(); // Ensure this is the correct function name
});

function loadInitialData() {
        getBonusprocessList();
        // Employee Type Change Handler
       
        // Load Employee Types
        $.ajax({
            url: '/api/Employees/GetEmployeeTypes',
            type: 'GET',
            dataType: 'json',
            success: function (responseObject) {
                const employeeTypes = responseObject.data;
                const $employeeTypeSelect = $('#EmployeeTypeId');
                $employeeTypeSelect.empty();
                $employeeTypeSelect.append(`<option value="0">Select One</option>`);
                $.each(employeeTypes, function (key, item) {
                    $employeeTypeSelect.append(`<option value="${item.id}">${item.employeeTypeName}</option>`);
                });
            },
            error: function () {
                alert('Failed to load employee types.');
            }
        });

        // Load Bonus Options
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

        // Load Months
        $.ajax({
            url: '/api/Others/GetMonths',
            type: 'GET',
            dataType: 'json',
            success: function (responseObject) {
                const months = responseObject.data;
                const currentMonth = new Date().getMonth() + 1;
                const $monthsSelect = $('#months');
                $monthsSelect.empty();
                $.each(months, function (key, item) {
                    const selected = currentMonth === parseInt(item.monthId) ? 'selected' : '';
                    $monthsSelect.append(`<option value="${item.monthId}" ${selected}>${item.monthName}</option>`);
                });
            },
            error: function () {
                alert('Failed to load months.');
            }
        });

        // Load Years
        $.ajax({
            url: '/api/Others/GetYears',
            type: 'GET',
            dataType: 'json',
            success: function (responseObject) {
                const years = responseObject.data;
                const currentYear = new Date().getFullYear();
                const $yearsSelect = $('#years');
                $yearsSelect.empty();
                $.each(years, function (key, item) {
                    const selected = parseInt(item) === currentYear ? 'selected' : '';
                    $yearsSelect.append(`<option value="${item}" ${selected}>${item}</option>`);
                });
            },
            error: function () {
                alert('Failed to load years.');
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



