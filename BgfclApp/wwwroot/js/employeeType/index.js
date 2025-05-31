let _dataTable;
let roles = [];

$(document).ready(function () {
    loadInitialData();
    let pageLength = parseInt($('#page_length_select').val());
    getEmployeeTypeList(pageLength);

    $('#page_length_select').on('change', function () {
        getEmployeeTypeList(parseInt($(this).val()));
    });

    $('#add_button').on('click', function () {
        resetForm();
        $('#operation_type').val('create');
        $('#submit_button').html('Create');
        $('#employeeTypeModalLabel').html('Add Employee Type');
        $('#employeeType_modal').modal('show');
    });

    $('#submit_button').on('click', submitEmployeeType);
    $('#cancel_button').on('click', function () {
        resetForm();
        $('#employeeType_modal').modal('hide');
    });
});

function resetForm() {
    $('#employeeTypeformId')[0].reset();
    $('#EmployeeTypeIdId').val('');
    $('.error-item').empty();
}

function submitEmployeeType() {
    console.log("click");
    let dataObj = {
        id: $('#EmployeeTypeId').val() || 0,
        userId: 0,
        schoolId: 0,
        roleId: $('#RoleId').val(),
        guestPkId: 0,
        employeeTypeName: $('#EmployeeType').val(),
        description: $('#Description').val(),
        isActive: $('#IsActive').is(":checked")
    };

    let url = $('#operation_type').val() === 'create' ? '/api/Employees/CreateEmployeeType' : '/api/Employees/UpdateEmployeeType';
    let type = $('#operation_type').val() === 'create' ? 'POST' : 'PUT';

    $.ajax({ url, type, data: dataObj })
        .always(handleResponse);
}

function handleResponse(responseObject) {
    if (responseObject.statusCode === 201 || responseObject.statusCode === 200) {
        resetForm();
        $('#employeeType_modal').modal('hide'); // Hide modal after successful update
        showToast('Success', responseObject.responseMessage, 'success');
        getEmployeeTypeList(parseInt($('#page_length_select').val()));
    } else {
        console.error("Error:", responseObject);
        showToast('Error', responseObject.responseMessage || 'Something went wrong!', 'error');
    }
}


function getEmployeeTypeList(pageLength, selectedType) {
    if (_dataTable) {
        _dataTable.destroy();
    }

    let url = selectedType && selectedType !== '0' ? `/api/Employees/GetEmployeeTypeById?id=${selectedType}` : '/api/Employees/GetEmployeeTypes';

    _dataTable = $('#employeeType_list_table').DataTable({
        pageLength,
        ajax: {
            url,
            dataSrc: function (json) {
                console.log("API Response: ", json); // Debugging: Log full API response
                if (!json || !json.data) {
                    console.warn("No data received from API.");
                    return [];
                }

                console.log("Processed Data for DataTable: ", json.data);
                return Array.isArray(json.data) ? json.data : [json.data];
            },
            error: function (xhr, error, thrown) {
                console.error("AJAX Error:", xhr.responseText);
            }
        },
        columns: [
            {
                data: null,
                className: 'text-center align-middle',
                render: (data, type, row, meta) => meta.row + meta.settings._iDisplayStart + 1
            },
            {
                data: 'employeeTypeName', orderable: false,
                className: 'text-center'
            },
            {
                data: 'isActive', orderable: false,
                className: 'text-center',
                render: data => data ? '<span class="text-success">Active</span>' : '<span class="text-danger">Inactive</span>'
            },
            {
                data: 'id', orderable: false,
                className: 'text-center align-middle',
                render: data => `
                    <button type="button" class="btn btn-primary btn-sm" onclick="onEditClicked(${data})">
                        <i class="fas fa-edit"></i>
                    </button> 
                    <button type="button" class="btn btn-danger btn-sm" onclick="onRemoveClicked(${data})">
                        <i class="fas fa-trash"></i>
                    </button>
                `
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
        initComplete: function () {
            let api = this.api();
            console.log("Table Initialized"); // Log when table is initialized

            // **Fix: Remove all existing filters and extra <br> tags to prevent header height increase**
            $('#employeeType_list_table thead th').find('.column-filter, br').remove();

            // Add column filters dynamically
            $('#employeeType_list_table thead th').each(function (index) {
                let title = $(this).text();
                if (index !== 0 && title && title !== 'Action') {
                    $(this).append(`<br><input type="text" class="form-control form-control-sm column-filter" placeholder=" " />`);
                }
            });

            // Apply column filtering
            api.columns().every(function () {
                let that = this;
                $('input.column-filter', this.header()).on('keyup change', function () {
                    console.log("Filtering Column:", that.index(), "Value:", this.value); // Log column filter inputs
                    if (that.search() !== this.value) {
                        that.search(this.value).draw();
                    }
                });
            });
        }
    });
}


function onEditClicked(EmployeeTypeId) {
    $('#employeeType_modal').modal('show');
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#employeeTypeModalLabel').html('Edit Employee Type');

    $.ajax({
        url: `/api/Employees/GetEmployeeTypeById?id=${EmployeeTypeId}`,
        type: 'GET',
        success: function (responseObject) {
            console.log("responseObject", responseObject)
            if (responseObject.statusCode === 200) {
                $('#EmployeeTypeId').val(responseObject.data.id);
                $('#EmployeeType').val(responseObject.data.employeeTypeName);
                $('#RoleId').val(responseObject.data.roleId);
                $('#Description').val(responseObject.data.description);
                $('#IsActive').prop('checked', responseObject.data.isActive);

            } else {
                showToast('Error', responseObject.responseMessage, 'error');
                $('#employeeType_modal').modal('hide');
            }
        },
        error: () => showToast('Error', 'An error occurred while fetching details.', 'error')
    });
}

function onRemoveClicked(employeeTypeId) {
    $('#remove_modal').modal('show');
    $('#EmployeeTypeId').val(employeeTypeId);
}

function onRemoveConfirmed() {
    $('#remove_spin').removeClass('d-none');
    let _employeeTypeId = $('#EmployeeTypeId').val();
    $('#remove_modal').modal('hide');
    $.ajax({ url: `/api/Employees/DeleteEmployeeType?id=${_employeeTypeId}`, type: 'DELETE', async: false })
        .always(responseObject => {
            if (responseObject.statusCode === 200) {
                getEmployeeTypeList(parseInt($('#page_length_select').val()));
                showToast('Success', responseObject.responseMessage, 'success');
            } else {
                showToast('Error', responseObject.responseMessage, 'error');
            }
        });
}

function loadInitialData() {
    $.ajax({
        url: '/api/Roles/GetRole',
        type: 'GET',
        dataType: 'json',
        success: function (responseObject) {
            roles = responseObject.data;
            $('#RoleId').empty().append('<option value="0">Select one</option>');
            roles.forEach(item => $('#RoleId').append(`<option value="${item.id}">${item.title}</option>`));
        }
    });
}
