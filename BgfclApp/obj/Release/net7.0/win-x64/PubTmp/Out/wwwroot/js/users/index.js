let _dataTable = undefined;

$(document).ready(function () {
    let pageLength = parseInt($('#page_length_select').val());

    getUserList(pageLength); // Initial load of the user list

    $('#page_length_select').on('change', function () {
        pageLength = parseInt($(this).val());
        getUserList(pageLength);
    });

    $('#filter_select').select2();

    $('#add_button').on('click', function () {
        $.ajax({
            url: '/api/Config/GetERPIntegrationStatus',
            type: 'GET',
            contentType: 'application/json',
            success: function (data) {
                if (data.isERPIntegrated) {
                    alert("You cannot perform this action because the system is integrated with ERP.");
                    return;
                }

                // Only proceed if ERP is NOT integrated
                $('#user_modal').modal('show');
                $('#UserId').val('');
                $('#Username').val('');
                $('#Password').val('');
                $('#Email').val('');
                $('#operation_type').val('create');
                $('#submit_button').html('Create');
                $('#userModalLabel').html('Add User');
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


    
    $('#submit_button').on('click', function () {
        let userId = parseInt($('#UserId').val()) || 0; 
        let username = $('#Username').val().trim();
        let password = $('#Password').val().trim();
        let email = $('#Email').val().trim();
        let isActive = $('#IsActive').is(":checked");
        let operationType = $('#operation_type').val();

        // Default values for optional fields
        let uuid = 0;
        let guestpkid = 0;
        let schoolid = 0;

        // Prepare the data object with the correct property name (userId instead of id)
        let dataObj = {
            userId: userId,
            uuId: uuid,
            guestPkId: guestpkid,
            schoolId: schoolid,
            username: username,
            password: password,
            email: email,
            isActive: isActive
        };

        // Determine the URL and HTTP method based on the operation type
        let url = operationType === 'create' ? '/api/Users/CreateUser' : '/api/Users/UpdateUser';
        let method = operationType === 'create' ? 'POST' : 'PUT';

        // Make the AJAX request
        $.ajax({
            url: url,
            type: method,
            contentType: 'application/json',
            data: JSON.stringify(dataObj),
            success: function (responseObject) {
                $('.error-item').empty();
                if (responseObject.statusCode === 200 || responseObject.statusCode === 201) {
                    $('#user_modal').modal('hide');
                    showToast('Success', responseObject.responseMessage, 'success');
                    getUserList(pageLength);
                } else {
                    showToast('Error', responseObject.responseMessage, 'error');
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
        $('#userformId')[0].reset();
        $('#user_modal').modal('hide');
    });
});


function getUserList(pageLength, selectedUser = null) {
    if (_dataTable) {
        _dataTable.destroy();
        _dataTable = null;
    }

    const url = selectedUser && selectedUser !== '0'
        ? `/api/Users/GetUserById?id=${selectedUser}`
        : '/api/Users/GetUser';

    _dataTable = $('#user_list_table').DataTable({
        pageLength: pageLength,
        ajax: {
            url: url,
            dataSrc: function (json) {
                console.log("API Response: ", json);
                return json && json.data ? (Array.isArray(json.data) ? json.data : [json.data]) : [];
            },
            error: function (xhr) {
                console.error("Error loading users:", xhr.responseText);
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
            { data: 'username', className: 'text-center', orderable: false },
            { data: 'password', className: 'text-center', orderable: false },
            { data: 'email', className: 'text-center', orderable: false },
            {
                data: 'isActive', orderable: false,
                className: 'text-center', orderable: false,
                render: function (data) {
                    return data ? '<span class="text-success">Active</span>' : '<span class="text-danger">Inactive</span>';
                }
            },
            {
                data: 'userId',
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
            $('#user_list_table thead th').find('.column-filter, br').remove();
            $('#user_list_table thead th').each(function (index) {
                let title = $(this).text();
                if (index !== 0 && title && title !== 'Action') {
                    $(this).append(`<br><input type="text" class="form-control form-control-sm column-filter" placeholder=" " />`);
                }
            });
            
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

function onEditClicked(userId) {
    userId = parseInt(userId, 10);
    if (isNaN(userId) || userId <= 0) {
        console.error("Invalid userId:", userId);
        showToast('Error', 'Invalid user ID.', 'error');
        return;
    }

    $('#user_modal').modal('show');
    console.log("Modal opened for userId:", userId);
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#userModalLabel').html('Edit User');
    $('#remove_spin').removeClass('d-none');

    $.ajax({
        url: `/api/Users/GetUserById?id=${userId}`,
        type: 'GET',
        success: function (responseObject) {
            console.log("API Response for Edit User:", responseObject);
            if (responseObject.statusCode === 200 && responseObject.data) {
                $('#UserId').val(responseObject.data.userId);
                $('#Username').val(responseObject.data.username);
                $('#Password').val(responseObject.data.password);
                $('#Email').val(responseObject.data.email);
                $('#IsActive').prop('checked', responseObject.data.isActive);
            } else {
                showToast('Error', responseObject.responseMessage || 'User not found.', 'error');
                $('#user_modal').modal('hide');
            }
        },
        error: function () {
            showToast('Error', 'An error occurred while fetching user details.', 'error');
            $('#user_modal').modal('hide');
        },
        complete: function () {
            $('#remove_spin').addClass('d-none');
        }
    });
}


function onRemoveClicked(userId) {
    $('#remove_modal').modal('show');
    $('#UserId').val(userId);
}

function onRemoveConfirmed() {
    $('#remove_spin').removeClass('d-none');
    let _userId = $('#UserId').val();
    $('#remove_modal').modal('hide');
    $.ajax({
        url: '/api/Users/DeleteUser?id=' + _userId,
        type: 'DELETE',
        async: false,
    }).always(function (responseObject) {
        if (responseObject.statusCode == 200) {
            getUserList();
            showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
        }
        if (responseObject.statusCode == 404 || responseObject.statusCode == 500) {
            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
        }
        $('#remove_spin').addClass('d-none');
    });
}

