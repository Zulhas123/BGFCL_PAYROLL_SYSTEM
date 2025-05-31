let _dataTable = undefined;
$(document).ready(function () {
    let pageLength = parseInt($('#page_length_select').val()) || 10;

    getRoleList(pageLength);

    $('#page_length_select').on('change', function () {
        pageLength = parseInt($(this).val());
        getRoleList(pageLength);
    });

    $('#filter_select').select2();

    $('#add_button').on('click', function () {
        $('#role_modal').modal('show');
        $('#roleformId')[0].reset(); // Reset form
        $('#operation_type').val('create');
        $('#submit_button').html('Create');
        $('#roleModalLabel').html('Add Role');
    });

    $('#submit_button').on('click', function () {
        console.log("Click")

        let roleId = parseInt($('#RoleId').val()) || 0;  
        let title = $('#Title').val().trim();
        let slug = $('#Slug').val().trim();
        let notes = $('#Notes').val().trim();

        let isEmployee = $('#is_employee').is(":checked");
        let isAuthority = $('#is_authority').is(":checked");
        let isStaff = $('#is_staff').is(":checked");
        let isActive = $('#Is_Active').is(":checked");

        let operationType = $('#operation_type').val();
        let userid = 0;
        let guestpkid = 0;
        let schoolid = 0;

        let dataObj = {
            id: roleId,   
            userId: userid,
            guestPkId: guestpkid,
            schoolId: schoolid,
            title: title,
            slug: slug,
            notes: notes,
            isEmployee: isEmployee,
            isAuthority: isAuthority,
            isStaff: isStaff,
            isActive: isActive
        };

        console.log("dataObj", dataObj);

        let url = operationType === 'create' ? '/api/Roles/CreateRole' : '/api/Roles/UpdateRole';
        let method = operationType === 'create' ? 'POST' : 'PUT';

        $.ajax({
            url: url,
            type: method,
            contentType: 'application/json',
            data: JSON.stringify(dataObj),
            success: function (response) {
                if (response.statusCode === 200 || response.statusCode === 201) {
                    $('#role_modal').modal('hide');
                    showToast('Success', response.responseMessage, 'success');
                    getRoleList(pageLength);
                } else {
                    showToast('Error', response.responseMessage, 'error');
                }
            },
            error: function (xhr) {
                let errorMessage = "An error occurred. Please try again.";
                if (xhr.responseJSON && xhr.responseJSON.responseMessage) {
                    errorMessage = xhr.responseJSON.responseMessage;
                }
                showToast('Error', errorMessage, 'error');
                console.error("Error response:", xhr.responseText);
            }
        });
    });


    $('#cancel_button').on('click', function () {
        $('#roleformId')[0].reset();
        $('#role_modal').modal('hide');
    });
});

function getRoleList(pageLength) {
    if (_dataTable) {
        _dataTable.destroy();
        _dataTable = null;
    }

    const url = '/api/Roles/GetRole';

    _dataTable = $('#roles_list_table').DataTable({
        pageLength: pageLength,
        ajax: {
            url: url,
            dataSrc: function (json) {
                return json && json.data ? (Array.isArray(json.data) ? json.data : [json.data]) : [];
            },
            error: function (xhr) {
                console.error("Error loading roles:", xhr.responseText);
            }
        },

        columns: [
            {
                data: null,
                className: 'text-center align-middle',
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: 'title', className: 'text-center', orderable: false },
            { data: 'slug', className: 'text-center', orderable: false },
            {
                data: null,
                className: 'text-center', orderable: false,
                render: function (data) {
                    if (data.isEmployee) {
                        return '<i class="fas fa-check-circle text-success"></i> Is Employee';
                    } else if (data.isAuthority) {
                        return '<i class="fas fa-check-circle text-success"></i> Is Authority';
                    } else if (data.isStaff) {
                        return '<i class="fas fa-check-circle text-success"></i> Is Staff';
                    } else {
                        return '<i class="fas fa-times-circle text-danger"></i> None';
                    }
                }
            },
            {
                data: 'isActive',
                className: 'text-center', orderable: false,
                render: function (data) {
                    return data ? '<span class="text-success">Active</span>' : '<span class="text-danger">Inactive</span>';
                }
            },
            {
                data: 'id',
                className: 'text-center align-middle', orderable: false,
                render: function (data) {
                    return `<button type="button" class="btn btn-primary btn-sm" onclick="onEditClicked(${data})">
                                <i class="fas fa-edit"></i>
                            </button> 
                            <button type="button" class="btn btn-danger btn-sm" onclick="onRemoveClicked(${data})">
                                <i class="fas fa-trash"></i>
                            </button>`;
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
        initComplete: function () {
            let api = this.api();

            // Clear existing filters
            $('#roles_list_table thead th .column-filter').remove();

            // Add input filters to each column except Action column
            $('#roles_list_table thead th').each(function (index) {
                let title = $(this).text();
                if (index !== 0 && title && title !== 'Action') {
                    $(this).append(`<br><input type="text" class="form-control form-control-sm column-filter" placeholder=" " />`);
                }
            });

            // Apply filtering logic
            api.columns().every(function () {
                let that = this;
                $('input.column-filter', this.header()).on('keyup change', function () {
                    if (that.search() !== this.value) {
                        that.search(this.value).draw(false);
                    }
                });
            });
        }
    });
}

function onEditClicked(roleId) {
    if (isNaN(roleId) || roleId <= 0) {
        console.error("Invalid roleId:", roleId);
        showToast('Error', 'Invalid role ID.', 'error');
        return;
    }

    console.log("Editing Role with ID:", roleId);

    $('#role_modal').modal('show');
    $('#roleformId')[0].reset(); // Reset form
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#roleModalLabel').html('Edit Role');

    $.ajax({
        url: `/api/Roles/GetRoleById?id=${roleId}`,
        type: 'GET',
        success: function (response) {
            if (response.statusCode === 200 && response.data) {
                $('#RoleId').val(response.data.id);  
                $('#Title').val(response.data.title);
                $('#Slug').val(response.data.slug);
                $('#Notes').val(response.data.notes);

                $('#is_employee').prop('checked', response.data.isEmployee);
                $('#is_authority').prop('checked', response.data.isAuthority);
                $('#is_staff').prop('checked', response.data.isStaff);
                $('#Is_Active').prop('checked', response.data.isActive);  // ✅ Fix: Correct checkbox ID
            } else {
                showToast('Error', response.responseMessage || 'Role not found.', 'error');
                $('#role_modal').modal('hide');
            }
        },
        error: function () {
            showToast('Error', 'An error occurred while fetching role details.', 'error');
            $('#role_modal').modal('hide');
        }
    });
}



function onRemoveClicked(roleId) {
    $('#remove_modal').modal('show');
    $('#RoleId').val(roleId);
}

function onRemoveConfirmed() {
    let roleId = $('#RoleId').val();
    $('#remove_modal').modal('hide');

    $.ajax({
        url: `/api/Roles/DeleteRole?id=${roleId}`,
        type: 'DELETE',
        success: function (response) {
            if (response.statusCode === 200) {
                $('#role_modal').modal('hide');
                showToast('Success', response.responseMessage, 'success');
                getRoleList(pageLength);
            } else {
                showToast('Error', response.responseMessage, 'error');
            }
        },
        error: function () {
            showToast('Error', 'An error occurred while deleting the role.', 'error');
        }
    });
}
