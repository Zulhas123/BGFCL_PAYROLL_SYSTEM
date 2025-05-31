let _dataTable = undefined;
$(document).ready(function () {
    let pageLength = parseInt($('#page_length_select').val());
    getDepartmentList(pageLength); // Initial load of the department list
    $('#filter_select').select2();
    // Open the modal form when clicking the "Add" button
    $('#add_button').on('click', function () {
        $('#department_modal').modal('show');
        $('#DepartmentId').val('');
        $('#operation_type').val('create');
        $('#submit_button').html('Create');
        $('#departmentModalLabel').html('Add Department');
    });

    // Fetch departments for the filter dropdown
    $.ajax({
        url: '/api/Departments/GetDepartments',
        method: 'GET',
        success: function (response) {
            var departments = response.data;
            departments.forEach(function (department) {
                $('#filter_select').append(
                    $('<option>', {
                        value: department.id,
                        text: department.departmentName
                    })
                );
            });
            // Add a default option for "All"
            $('#filter_select').prepend($('<option>', { value: '', text: 'All Departments' }));
        },
        error: function (xhr, status, error) {
            console.error('Failed to fetch departments:', error);
        }
    });

    // When a department is selected, filter the department list
    $('#filter_select').on('change', function () {
        var selectedDepartment = $(this).val(); // Get the selected value
        console.log("Selected Department ID: ", selectedDepartment); // Debug log
        getDepartmentList(pageLength, selectedDepartment); // Pass the selected department ID
    });

    $('#submit_button').on('click', function () {
        let departmentName = $('#DepartmentName').val();
        /*let journalCode = $('#JournalCode').val();*/
        let description = $('#Description').val();
        let departmentId = $('#DepartmentId').val() || 0; // Default to 0 if empty
        let isActive = $('#IsActive').is(":checked");

        let dataObj = {
            id: departmentId,
            departmentName: departmentName,
            journalCode: null,
            description: description,
            isActive: isActive
        };

        let operationType = $('#operation_type').val();

        if (operationType === 'create') {
            $.ajax({
                url: '/api/Departments/CreateDepartment',
                type: 'POST',
                data: dataObj
            }).always(function (responseObject) {
                //handleResponse(responseObject);
                $('.error-item').empty();
                if (responseObject.statusCode == 201) {
                    $('#DepartmentName').val('');
                    /*$('#JournalCode').val('');*/
                    $('#Description').val('');
                    $('#department_modal').modal('hide');
                    showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    getDepartmentList(pageLength);
                }
            });
        } else {
            $.ajax({
                url: '/api/Departments/UpdateDepartment',
                type: 'PUT',
                data: dataObj
            }).always(function (responseObject) {
                //handleResponse(responseObject);
                $('.error-item').empty();
                if (responseObject.statusCode == 201) {
                    $('#DepartmentName').val('');
                    /*$('#JournalCode').val('');*/
                    $('#Description').val('');
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

function getDepartmentList(pageLength, selectedDepartment) {
    if (_dataTable !== undefined) {
        _dataTable.destroy();
    }


    // Determine the URL based on the selectedDepartment
    const url = selectedDepartment && selectedDepartment !== '0'
        ? `/api/Departments/GetDepartmentById?id=${selectedDepartment}`
        : '/api/Departments/GetDepartments';

    _dataTable = $('#department_list_table').DataTable({
        pageLength: pageLength,
        ajax: {
            url: url,
            dataSrc: function (json) {
                console.log("API Response: ", json); // Debug log of the response
                // Check if we received a single object and convert it to an array
                return Array.isArray(json.data) ? json.data : [json.data];
            },
            data: function (d) {
                d.departmentId = selectedDepartment; // Pass selected department ID for filtering (if needed)
                console.log("Department ID sent to server: ", selectedDepartment); // Debug log
            },
        },
        columns: [
            {
                data: '',
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: 'departmentName' },
            /*{ data: 'journalCode' },*/
            {
                data: 'isActive',
                render: function (data) {
                    return data ? 'Active' : 'Inactive';
                }
            },
            {
                data: 'id',
                render: function (data, type, row) {
                    return `<button type="button" class="btn btn-primary btn-sm" onclick="onEditClicked(${data})">Edit</button> 
                            <button type="button" class="btn btn-danger btn-sm" onclick="onRemoveClicked(${data})">Delete</button>`;
                }
            },
        ],
        dom: '<"d-flex justify-content-between align-items-center"Bf>tip',
        buttons: [
            'copy', 'excel', 'pdf', 'print'
        ]
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
        type: 'GET',
        success: function (responseObject) {
            if (responseObject.statusCode === 200) {
                // Populate the form fields with the retrieved data
                $('#DepartmentId').val(responseObject.data.id);
                $('#DepartmentName').val(responseObject.data.departmentName);
                /*$('#JournalCode').val(responseObject.data.journalCode);*/
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

