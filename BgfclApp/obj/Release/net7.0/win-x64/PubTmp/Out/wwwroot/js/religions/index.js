let _dataTable = undefined;
let roles = [];
$(document).ready(function () {

    let pageLength = parseInt($('#page_length_select').val());
    getReligionList(pageLength); 
    // Open the modal form when clicking the "Add" button
    $('#add_button').on('click', function () {
        $('#religion_modal').modal('show');
        $('#ReligionId').val('');
        $('#operation_type').val('create');
        $('#submit_button').html('Create');
        $('#religionModalLabel').html('Add Religion');
        $('#religionformId')[0].reset(); // Reset the form
        $('.error-item').empty(); 
    });

    $('#submit_button').on('click', function () {
        let roleId = $('#RoleId').val();
        let religionName = $('#ReligionName').val();
        let description = $('#Description').val();
        let religionId = $('#ReligionId').val() || 0;
        let isActive = $('#IsActive').is(":checked");

        let userId = 0;
        let schoolId = 0;
        let guestPkId = 0;
        let dataObj = {
            id: religionId,
            userId: userId,
            schoolId: schoolId,
            RoleId: roleId,
            guestPkId: guestPkId,
            religionName: religionName,
            description: description,
            isActive: isActive
        };

        let operationType = $('#operation_type').val();
        let ajaxUrl = operationType === 'create' ? '/api/Religions/CreateReligion' : '/api/Religions/UpdateReligion';
        let ajaxType = operationType === 'create' ? 'POST' : 'PUT';

        $.ajax({
            url: ajaxUrl,
            type: ajaxType,
            async: false,
            data: dataObj
        }).always(function (responseObject) {
            $('.error-item').empty();
            if (responseObject.statusCode == 201) {
                $('#ReligionName').val('');
                $('#Description').val('');
                $('#RoleId').val('');
                $('#religion_modal').modal('hide');
                showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                getReligionList(pageLength);
            }
        });

        console.log("obj", data)
    });

    $('#cancel_button').on('click', function () {
        resetReligionModal(); // Reset the form and hide modal
        $('#religion_modal').modal('hide');
    });

    $('#page_length_select').on('change', function () {
        pageLength = parseInt($(this).val());
        getReligionList(pageLength);
    });
});
function resetReligionModal() {
    $('#religion_modal').modal('show');
    $('#ReligionId').val('');
    $('#operation_type').val('create');
    $('#submit_button').html('Create');
    $('#religionModalLabel').html('Add Religion');
    $('#religionformId')[0].reset(); // Reset the form
    $('.error-item').empty(); // Clear error messages
}


// Function to fetch and display the list of designations
function getReligionList(pageLength, selectedReligion) {
    if (_dataTable !== undefined) {
        _dataTable.destroy();
    }

    const url = selectedReligion && selectedReligion !== '0'
        ? `/api/Religions/GetReligionById?id=${selectedReligion}`
        : '/api/Religions/GetReligions';

    console.log("Fetching Data from URL:", url); // Log API endpoint

    _dataTable = $('#religion_list_table').DataTable({
        pageLength: pageLength,
        ajax: {
            url: url,
            dataSrc: function (json) {
                console.log("API Response Data: ", json); // Debug API response
                return Array.isArray(json.data) ? json.data : [json.data]; // Handle both object and array response
            },
            data: function (d) {
                d.religionId = selectedReligion;
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
            { data: 'religionName', className: 'text-center',orderable: false },
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
            $('#religion_list_table thead th').find('.column-filter, br').remove();

            // Add column filters dynamically
            $('#religion_list_table thead th').each(function (index) {
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


function onEditClicked(religionId) {
    // Show the modal for editing
    $('#religion_modal').modal('show');

    // Set the operation type to 'edit' and update the button text
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#religionModalLabel').html('Edit Religion');

    // Show a loading indicator or spinner if needed
    $('#remove_spin').removeClass('d-none');

    $.ajax({
        url: '/api/Religions/GetReligionById?id=' + religionId,
        type: 'GET',
        success: function (responseObject) {
            console.log("responseObject", responseObject)
            if (responseObject.statusCode === 200) {
                // Populate the form fields with the retrieved data
                $('#ReligionId').val(responseObject.data.id);
                $('#ReligionName').val(responseObject.data.religionName);
                $('#Description').val(responseObject.data.description);
                $('#IsActive').prop('checked', responseObject.data.isActive);
            } else if (responseObject.statusCode === 404) {
                showToast('Error', responseObject.responseMessage, 'error');
                $('#religion_modal').modal('hide');
            }
        },
        error: function () {
            showToast('Error', 'An error occurred while fetching designation details.', 'error');
            $('#religion_modal').modal('hide');
        },
        complete: function () {
            $('#remove_spin').addClass('d-none');
        }
    });
}

function onRemoveClicked(religionId) {
    $('#remove_modal').modal('show');
    $('#ReligionId').val(religionId);
}

function onRemoveConfirmed() {
    $('#remove_spin').removeClass('d-none');
    let _religiond = $('#ReligionId').val();
    $('#remove_modal').modal('hide');
    $.ajax({
        url: '/api/Religions/DeleteReligion?id=' + _religiond,
        type: 'DELETE',
        async: false,
    }).always(function (responseObject) {
        if (responseObject.statusCode == 200) {
            getReligionList();
            showToast('Success', responseObject.responseMessage, 'success');
        }
        if (responseObject.statusCode == 404 || responseObject.statusCode == 500) {
            showToast('Error', responseObject.responseMessage, 'error');
        }
        $('#remove_spin').addClass('d-none');
    });
}


