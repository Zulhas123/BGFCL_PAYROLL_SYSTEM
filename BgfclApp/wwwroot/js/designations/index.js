let _dataTable = undefined;
let roles = [];
$(document).ready(function () {

    let pageLength = parseInt($('#page_length_select').val());
    getDesignationList(pageLength); 
    initialLoads(); 
    // Open the modal form when clicking the "Add" button
    $('#add_button').on('click', function () {
        $('#designation_modal').modal('show');
        $('#DesignationId').val('');
        $('#operation_type').val('create');
        $('#submit_button').html('Create');
        $('#designationModalLabel').html('Add Designation');
        $('#designationForm')[0].reset(); // Reset the form
        $('.error-item').empty(); 
    });

    $('#submit_button').on('click', function () {
        let roleId = $('#RoleId').val();
        let employeeTypeId = parseInt($('#EmployeeTypeId').val());
        let designationName = $('#DesignationName').val();
        let description = $('#Description').val();
        let designationId = $('#DesignationId').val() || 0;
        let isActive = $('#IsActive').is(":checked");

        let userId = 0;
        let schoolId = 0;
        let guestPkId = 0;
        let multiDesignation = '';
        let dataObj = {
            id: designationId,
            userId: userId,
            schoolId: schoolId,
            RoleId: roleId,
            guestPkId: guestPkId,
            multiDesignation: multiDesignation,
            designationName: designationName,
            description: description,
            employeeTypeId: employeeTypeId,
            isActive: isActive
        };

        let operationType = $('#operation_type').val();
        let ajaxUrl = operationType === 'create' ? '/api/Designations/CreateDesignation' : '/api/Designations/UpdateDesignation';
        let ajaxType = operationType === 'create' ? 'POST' : 'PUT';

        $.ajax({
            url: ajaxUrl,
            type: ajaxType,
            async: false,
            data: dataObj
        }).always(function (responseObject) {
            $('.error-item').empty();
            if (responseObject.statusCode == 201) {
                $('#EmployeeTypeId').val('');
                $('#DesignationName').val('');
                $('#Description').val('');
                $('#RoleId').val('');
                $('#designation_modal').modal('hide');
                showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                getDesignationList(pageLength);
            }
        });

        console.log("obj", data)
    });
    //$('#cancel_button').on('click', () => {
    //    $('#designation_modal')[0].reset();
    //    $('#DesignationNameError').text('');
    //    $('#department_modal').modal('hide');
    //});
    $('#cancel_button').on('click', function () {
        resetDesignationModal(); // Reset the form and hide modal
        $('#designation_modal').modal('hide');
    });

    $('#page_length_select').on('change', function () {
        pageLength = parseInt($(this).val());
        getDesignationList(pageLength);
    });
});
function resetDesignationModal() {
    $('#designation_modal').modal('show');
    $('#DesignationId').val('');
    $('#operation_type').val('create');
    $('#submit_button').html('Create');
    $('#designationModalLabel').html('Add Designation');
    $('#designationForm')[0].reset(); // Reset the form
    $('.error-item').empty(); // Clear error messages
}


// Function to fetch and display the list of designations
function getDesignationList(pageLength, selectedDesignation) {
    if (_dataTable !== undefined) {
        _dataTable.destroy();
    }

    const url = selectedDesignation && selectedDesignation !== '0'
        ? `/api/Designations/GetDesignationById?id=${selectedDesignation}`
        : '/api/Designations/GetDesignations';

    console.log("Fetching Data from URL:", url); // Log API endpoint

    _dataTable = $('#designation_list_table').DataTable({
        pageLength: pageLength,
        ajax: {
            url: url,
            dataSrc: function (json) {
                console.log("API Response Data: ", json); // Debug API response
                return Array.isArray(json.data) ? json.data : [json.data]; // Handle both object and array response
            },
            data: function (d) {
                d.designationId = selectedDesignation;
                console.log("Data Sent to Server: ", d); // Log request parameters
            },
        },
        columns: [
            {
                data: '',
                className: 'text-center align-middle',
                render: function (data, type, row, meta) {
                    console.log("Row Data:", row); // Log each row's data
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: 'designationName', className: 'text-center',orderable: false },
            { data: 'employeeTypeName', className: 'text-center',orderable: false },
            {
                data: 'roleId',
                className: 'text-center', orderable: false,
                render: function (data) {
                    console.log("Current RoleId:", data);
                    console.log("Roles Array:", roles);

                    let role = roles.find(r => r.id == data);
                    console.log("Matched Role:", role);

                    return role ? role.title : 'N/A';
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
                    console.log("Action Column Data (ID):", data);
                    return `<button type="button" class="btn btn-primary btn-sm" onclick="onEditClicked(${data})"><i class="fas fa-edit"></i></button> 
                            <button type="button" class="btn btn-danger btn-sm" onclick="onRemoveClicked(${data})"> <i class="fas fa-trash"></i></button>`;
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
            console.log("Table Initialized"); // Log when table is initialized

            // **Fix: Remove all existing filters and extra <br> tags to prevent header height increase**
            $('#designation_list_table thead th').find('.column-filter, br').remove();

            // Add column filters dynamically
            $('#designation_list_table thead th').each(function (index) {
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


function onEditClicked(designationId) {
    // Show the modal for editing
    $('#designation_modal').modal('show');

    // Set the operation type to 'edit' and update the button text
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#designationModalLabel').html('Edit Designation');

    // Show a loading indicator or spinner if needed
    $('#remove_spin').removeClass('d-none');

    // Fetch the designation data based on the designationId
    $.ajax({
        url: '/api/Designations/GetDesignationById?id=' + designationId,
        type: 'GET',
        success: function (responseObject) {
            console.log("responseObject", responseObject)
            if (responseObject.statusCode === 200) {
                // Populate the form fields with the retrieved data
                $('#DesignationId').val(responseObject.data.id);
                $('#EmployeeTypeId').val(responseObject.data.employeeTypeId);
                $('#DesignationName').val(responseObject.data.designationName);
                $('#RoleId').val(responseObject.data.roleId);
                $('#Description').val(responseObject.data.description);
                $('#IsActive').prop('checked', responseObject.data.isActive);
            } else if (responseObject.statusCode === 404) {
                showToast('Error', responseObject.responseMessage, 'error');
                $('#designation_modal').modal('hide');
            }
        },
        error: function () {
            showToast('Error', 'An error occurred while fetching designation details.', 'error');
            $('#designation_modal').modal('hide');
        },
        complete: function () {
            $('#remove_spin').addClass('d-none');
        }
    });
}

function onRemoveClicked(designationId) {
    $('#remove_modal').modal('show');
    $('#DesignationId').val(designationId);
}

function onRemoveConfirmed() {
    $('#remove_spin').removeClass('d-none');
    let _designationId = $('#DesignationId').val();
    $('#remove_modal').modal('hide');
    $.ajax({
        url: '/api/Designations/RemoveDesignation?id=' + _designationId,
        type: 'DELETE',
        async: false,
    }).always(function (responseObject) {
        if (responseObject.statusCode == 200) {
            getDesignationList();
            showToast('Success', responseObject.responseMessage, 'success');
        }
        if (responseObject.statusCode == 404 || responseObject.statusCode == 500) {
            showToast('Error', responseObject.responseMessage, 'error');
        }
        $('#remove_spin').addClass('d-none');
    });
}

function initialLoads() {
    $.ajax({
        url: '/api/Employees/GetEmployeeTypes',
        type: 'GET',
        dataType: 'json',
        success: function (responseObject) {
            let employeeTypes = responseObject.data;
            $('#EmployeeTypeId').empty(); // Clear the dropdown
            $('#EmployeeTypeId').append('<option value="0">Select one</option>'); // Default option

            $.each(employeeTypes, function (key, item) {
                $('#EmployeeTypeId').append(`<option value="${item.id}">${item.employeeTypeName}</option>`);
            });
        },
        error: function (responseObject) {
            showToast('Error', 'Failed to load employee types', 'error');
        }
    });
    $.ajax({
        url: '/api/Roles/GetRole',
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

