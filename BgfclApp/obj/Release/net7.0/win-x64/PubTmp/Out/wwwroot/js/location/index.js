let _dataTable = undefined;
$(document).ready(function () {
    let pageLength = parseInt($('#page_length_select').val());
    // Open the modal form when clicking the "Add" button
    $('#add_button').on('click', function () {
        $('#location_modal').modal('show');
        $('#LocationId').val('');
        $('#operation_type').val('create');
        $('#submit_button').html('Create');
        $('#locationModalLabel').html('Add Location');
    });
    getLocationList(pageLength);

    $('#submit_button').on('click', function () {
        let locationName = $('#LocationName').val();
        let locationId = $('#LocationId').val();
        let isActive = $('#IsActive').is(":checked");
        if (locationId == '') {
            locationId = 0;
        }

        let dataObj = {
            id: locationId,
            locationName: locationName,
            isActive: isActive
           
        };

        let operationType = $('#operation_type').val();

        if (operationType == 'create') {
            $.ajax({
                url: '/api/Locations/CreateLocation',
                type: 'POST',
                async: false,
                data: dataObj
            }).always(function (responseObject) {
                $('.error-item').empty();
                if (responseObject.statusCode == 201) {
                    $('#LocationName').val('');
                    $('#locationForm')[0].reset();
                    $('#location_modal').modal('hide');
                    showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    getLocationList();
                }
                if (responseObject.statusCode == 400 || responseObject.statusCode == 409) {
                    for (let error in responseObject.errors) {
                        $(`#${error}`).empty();
                        $(`#${error}`).append(responseObject.errors[error]);
                    }
                    showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                }
            });


        }
        else {

            $.ajax({
                url: '/api/Locations/UpdateLocation',
                type: 'PUT',
                async: false,
                data: dataObj
            }).always(function (responseObject) {
                $('.error-item').empty();
                if (responseObject.statusCode == 201) {
                    $('#LocationName').val('');
                    $('#locationForm')[0].reset();
                    $('#location_modal').modal('hide');

                    showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    getLocationList();
                    $('#operation_type').val('create');
                    $('#submit_button').html('Create');
                }
                if (responseObject.statusCode == 400) {
                    for (let error in responseObject.errors) {
                        $(`#${error}`).empty();
                        $(`#${error}`).append(responseObject.errors[error]);
                    }
                    showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                }
                if (responseObject.statusCode == 500) {
                    showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                }
            });

        }


    });
    $('#cancel_button').on('click', () => {
        location.reload();
    });
    $('#page_length_select').on('change', function () {
        pageLength = parseInt($(this).val());
        getLocationList(pageLength);
    });
});

function getLocationList(pageLength) {


    if (_dataTable != undefined) {
        _dataTable.destroy();
    }

    _dataTable = $('#location_list_table').DataTable({
        pageLength: pageLength,
        ajax: {
            url: '/api/Locations/GetLocations',
            dataSrc: 'data'
        },
        columns: [
            {
                data: '',
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: 'locationName' },
            {
                data: 'isActive',
                render: (data, type) => {
                    if (data.toString() == 'true') {
                        return 'active';
                    }
                    if (data.toString() == 'false') {
                        return 'InActive';
                    }
                    return data;
                }
            },
            {
                data: 'id',
                render: (data, type, row) => {
                    return `<button type="button" class="btn btn-primary btn-sm" onclick="onEditClicked(${data})">edit</button> <button type="button" class="btn btn-danger btn-sm" onclick="onRemoveClicked(${data})">delete</button>`;
                }
            },

        ],
        dom: '<"d-flex justify-content-between align-items-center"Bf>tip',
        buttons: [
            'copy', 'excel', 'pdf', 'print'
        ]
    });
}


//function onEditClicked(locationId) {
//    $('#edit_modal').modal('show');
//    $('#LocationId').val(locationId);
//}

function onEditClicked(locationId) {
    // Show the modal for editing
    $('#location_modal').modal('show');

    // Set the operation type to 'edit' and update the button text
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#locationModalLabel').html('Edit Location');

    // Show a loading indicator or spinner if needed
    $('#remove_spin').removeClass('d-none');

    // Fetch the location data based on the locationId
    $.ajax({
        url: '/api/Locations/GetLocationById?id=' + locationId,
        type: 'GET',
        async: false,
    }).always(function (responseObject) {
        if (responseObject.statusCode == 200) {
            // Populate the form fields with the retrieved data
            $('#LocationId').val(responseObject.data.id);
            $('#LocationName').val(responseObject.data.locationName);
            $('#IsActive').prop('checked', responseObject.data.isActive);
        } else if (responseObject.statusCode == 404) {
            showToast('Error', responseObject.responseMessage, 'error');
            $('#location_modal').modal('hide');
        } else {
            showToast('Error', 'An unexpected error occurred.', 'error');
            $('#location_modal').modal('hide');
        }
        $('#remove_spin').addClass('d-none');
    });
}



//function onEditConfirmed() {
//    $('#remove_spin').removeClass('d-none');
//    let _locationId = $('#LocationId').val();
//    $('#operation_type').val('edit');
//    $('#submit_button').html('Save');
//    $('#edit_modal').modal('hide');
//    $.ajax({
//        url: '/api/Locations/GetLocationById?id=' + _locationId,
//        type: 'GET',
//        async: false,
//    }).always(function (responseObject) {
//        if (responseObject.statusCode == 200) {
//            $('#LocationName').val(responseObject.data.locationName);
//            $('#IsActive').prop('checked', responseObject.data.isActive);
          
//        }
//        if (responseObject.statusCode == 404) {
//            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
//        }
//        $('#remove_spin').addClass('d-none');
//    });
//}

function onRemoveClicked(locationId) {
    $('#remove_modal').modal('show');
    $('#LocationId').val(locationId);
}

function onRemoveConfirmed() {
    $('#remove_spin').removeClass('d-none');
    let _locationId = $('#LocationId').val();
    $('#remove_modal').modal('hide');
    $.ajax({
        url: '/api/Locations/RemoveLocation?id=' + _locationId,
        type: 'DELETE',
        async: false,
    }).always(function (responseObject) {
        if (responseObject.statusCode == 200) {
            getLocationList();
            $('#LocationName').val('');
          
            $('#operation_type').val('create');
            $('#submit_button').html('create');

            showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
        }
        if (responseObject.statusCode == 404 || responseObject.statusCode == 500) {
            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
        }
        $('#remove_spin').addClass('d-none');
    });
}


