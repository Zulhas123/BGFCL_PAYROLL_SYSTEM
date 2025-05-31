let _dataTable = undefined;
let roles = [];
let auth_token = '';
$(document).ready(function () {

    // reading cookie
    auth_token = getCookie('bgfcl_auth_token');


    loadInitialData();
    let pageLength = parseInt($('#page_length_select').val());

    getDepartmentList(pageLength); // Initial load of the department list
    $('#page_length_select').on('change', function () {
        pageLength = parseInt($(this).val());
        getDepartmentList(pageLength);
    });

    $('#add_button').on('click', function () {
        $('#department_modal').modal('show');
        $('#DepartmentId').val('');
        $('#operation_type').val('create');
        $('#submit_button').html('Create');
        $('#departmentModalLabel').html('Add Department');
    });


    $('#submit_button').on('click', function () {
        let roleId = $('#RoleId').val();
        let departmentName = $('#DepartmentName').val();
        let journalCode = $('#JournalCode').val();
        let description = $('#Description').val();
        let departmentId = $('#DepartmentId').val() || 0; // Default to 0 if empty
        let isActive = $('#IsActive').is(":checked");

        let userId = 0;
        let schoolId = 0;
       // let roleId = 0;
        let guestPkId = 0;

        let dataObj = {
            id: departmentId,
            userId: userId,
            schoolId: schoolId,
            roleId: roleId,
            guestPkId: guestPkId,
            departmentName: departmentName,
            journalCode: journalCode,
            description: description,
            isActive: isActive
        };

        let operationType = $('#operation_type').val();

        if (operationType === 'create') {
            $.ajax({
                url: '/api/Departments/CreateDepartment',
                headers: { 'Authorization': 'Bearer ' + auth_token },
                type: 'POST',
                data: dataObj
            }).always(function (responseObject) {
                //handleResponse(responseObject);
                $('.error-item').empty();
                if (responseObject.statusCode == 201) {
                    $('#DepartmentName').val('');
                    $('#JournalCode').val('');
                    $('#Description').val('');
                    $('#RoleId').val('');
                    $('#department_modal').modal('hide');
                    showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    getDepartmentList(pageLength);
                }
            });
        } else {
            $.ajax({
                url: '/api/Departments/UpdateDepartment',
                headers: { 'Authorization': 'Bearer ' + auth_token },
                type: 'PUT',
                data: dataObj
            }).always(function (responseObject) {
                //handleResponse(responseObject);
                $('.error-item').empty();
                if (responseObject.statusCode == 201) {
                    $('#DepartmentName').val('');
                    $('#JournalCode').val('');
                    $('#Description').val('');
                    $('#RoleId').val('');
                    $('#department_modal').modal('hide');
                    showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    getDepartmentList(pageLength);
                }

            });
        }
    });

    $('#cancel_button').on('click', () => {
        $('#departmentformId')[0].reset();
        $('#DepartmentNameError').text('');
        $('#department_modal').modal('hide');
    });

    $('#page_length_select').on('change', function () {
        pageLength = parseInt($(this).val());
        getDepartmentList(pageLength);
    });
});


function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}
function getDepartmentList(pageLength, selectedDepartment) {
    if (_dataTable !== undefined) {
        _dataTable.destroy();
        _dataTable = null;
    }

    // Determine the API URL based on selectedDepartment
    const url = selectedDepartment && selectedDepartment !== '0'
        ? `/api/Departments/GetDepartmentById?id=${selectedDepartment}`
        : '/api/Departments/GetDepartments';

    _dataTable = $('#department_list_table').DataTable({
        pageLength: pageLength,
        ajax: {
            url: url,
            headers: { 'Authorization': 'Bearer '+auth_token },
            dataSrc: function (json) {
                console.log("API Response: ", json); // Debug log
                if (!json || !json.data) return []; // Ensure it does not break if data is null/undefined
                return Array.isArray(json.data) ? json.data : [json.data];
            },
            error: function (xhr, error, thrown) {
                console.error("Error loading departments:", xhr.responseText);
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
            { data: 'departmentName', className: 'text-center', orderable: false },
            {
                data: 'roleId', orderable: false,
                className: 'text-center',
                render: function (data) {
                    console.log("Current RoleId:", data);
                    console.log("Roles array:", roles);

                    let role = roles.find(r => r.id == data);
                    console.log("Matched Role:", role);

                    return role ? role.title : 'N/A';
                }
            },

            {
                data: 'isActive', orderable: false,
                className: 'text-center',
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
            },
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

            // **Fix: Remove all existing filters and extra <br> tags to prevent header height increase**
            $('#department_list_table thead th').find('.column-filter, br').remove();

            // Add column filters dynamically
            $('#department_list_table thead th').each(function (index) {
                let title = $(this).text();
                if (index !== 0 && title && title !== 'Action') {
                    $(this).append(`<br><input type="text" class="form-control form-control-sm column-filter" placeholder=" " />`);
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
    });
}




function onEditClicked(departmentId) {
    // Show the modal for editing
    $('#department_modal').modal('show');

    // Set the operation type to 'edit' and update the button text
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#departmentModalLabel').html('Edit Department');

    // Show a loading indicator or spinner if needed
    $('#remove_spin').removeClass('d-none');

    // Fetch the department data based on the departmentId
    $.ajax({
        url: '/api/Departments/GetDepartmentById?id=' + departmentId,
        headers: { 'Authorization': 'Bearer ' + auth_token },
        type: 'GET',
        success: function (responseObject) {
            if (responseObject.statusCode === 200) {
                // Populate the form fields with the retrieved data
                $('#DepartmentId').val(responseObject.data.id);
                $('#DepartmentName').val(responseObject.data.departmentName);
                $('#RoleId').val(responseObject.data.roleId);
                $('#JournalCode').val(responseObject.data.journalCode);
                $('#Description').val(responseObject.data.description);
                $('#IsActive').prop('checked', responseObject.data.isActive);
            } else if (responseObject.statusCode === 404) {
                showToast('Error', responseObject.responseMessage, 'error');
                $('#department_modal').modal('hide');
            }
        },
        error: function () {
            showToast('Error', 'An error occurred while fetching department details.', 'error');
            $('#department_modal').modal('hide');
        },
        complete: function () {
            $('#remove_spin').addClass('d-none');
        }
    });
}






function onRemoveClicked(departmentId) {
    $('#remove_modal').modal('show');
    $('#DepartmentId').val(departmentId);
}

function onRemoveConfirmed() {
    $('#remove_spin').removeClass('d-none');
    let _departmentId = $('#DepartmentId').val();
    $('#remove_modal').modal('hide');
    $.ajax({
        url: '/api/Departments/RemoveDepartment?id=' + _departmentId,
        headers: { 'Authorization': 'Bearer ' + auth_token },
        type: 'DELETE',
        async: false,
    }).always(function (responseObject) {
        if (responseObject.statusCode == 200) {
            getDepartmentList();
            showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
        }
        if (responseObject.statusCode == 404 || responseObject.statusCode == 500) {
            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
        }
        $('#remove_spin').addClass('d-none');
    });
}

function loadInitialData() {
    
    $.ajax({
        url: '/api/Roles/GetRole',
        headers: { 'Authorization': 'Bearer ' + auth_token },
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            roles = responseObject.data;
            $('#RoleId').empty();
            $('#RoleId').append('<option value="0">select one</option>');
            $.each(roles, function (key, item) {
                $('#RoleId').append(`<option value=${item.id}>${item.title}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });


}