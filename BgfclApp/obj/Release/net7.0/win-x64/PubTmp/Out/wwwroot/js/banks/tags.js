let _dataTable = undefined;
$(document).ready(function () {
    let pageLength = parseInt($('#page_length_select').val());
    GetBankTagList(pageLength);
    $('#add_button').on('click', function () {
        $('#bank_tag_modal').modal('show');
        $('#BankTagId').val('');
        $('#operation_type').val('create');
        $('#submit_button').html('Create');
        $('#bankTagModalLabel').html('Add Bank Tag');
    });

    $('#submit_button').on('click', function () {
        let bankTagName = $('#BankTagName').val();
        let description = $('#Description').val();
        let bankTagId = $('#BankTagId').val();
        let isActive = $('#IsActive').is(":checked");
        if (bankTagId == '') {
            bankTagId = 0;
        }

        let dataObj = {
            id: bankTagId,
            bankTagName: bankTagName,
            description: description,
            isActive: isActive
        };

        let operationType = $('#operation_type').val();

        if (operationType == 'create') {


            $.ajax({
                url: '/api/Banks/CreateBankTag',
                type: 'POST',
                async: false,
                data: dataObj
            }).always(function (responseObject) {
                $('.error-item').empty();
                if (responseObject.statusCode == 201) {
                    $('#BankTagName').val('');
                    $('#Description').val('');
                    $('#tagForm')[0].reset();
                    $('#bank_tag_modal').modal('hide');
                    showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    GetBankTagList();
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
                url: '/api/Banks/UpdateBankTag',
                type: 'PUT',
                async: false,
                data: dataObj
            }).always(function (responseObject) {
                $('.error-item').empty();
                if (responseObject.statusCode == 201) {
                    $('#BankTagName').val('');
                    $('#Description').val('');
                    $('#tagForm')[0].reset();
                    $('#bank_tag_modal').modal('hide');
                    showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    GetBankTagList();
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
        GetBankTagList(pageLength);
    });

});

function GetBankTagList(pageLength) {

    if (_dataTable != undefined) {
        _dataTable.destroy();
    }


    _dataTable = $('#bank_tag_list_table').DataTable({
        pageLength: pageLength,
        ajax: {
            url: '/api/Banks/GetBankTags',
            dataSrc: 'data'
        },
        columns: [
            {
                data: '',
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: 'bankTagName' },
            {
                data: 'isActive',
                render: (data, type) => {
                    if (data.toString() == 'true') {
                        return 'active';
                    }
                    if (data.toString() == 'false') {
                        return 'Inactive';
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

//function onEditClicked(bankTagId) {
//    $('#edit_modal').modal('show');
//    $('#BankTagId').val(bankTagId);
//}


function onEditClicked(bankTagId) {
    // Show the modal for editing
    $('#bank_tag_modal').modal('show');

    // Set the operation type to 'edit' and update the button text
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#bankTagModalLabel').html('Edit Bank Tag');

    // Show a loading indicator or spinner if needed
    $('#remove_spin').removeClass('d-none');

    // Fetch the bank tag data based on the bankTagId
    $.ajax({
        url: '/api/Banks/GetBankTagById?id=' + bankTagId, // Use the correct variable bankTagId
        type: 'GET',
        success: function (responseObject) {
            if (responseObject.statusCode === 200) {
                console.log("DATA", responseObject);
                $('#BankTagId').val(responseObject.data.id);
                $('#BankTagName').val(responseObject.data.bankTagName);
                $('#Description').val(responseObject.data.description);
                $('#IsActive').prop('checked', responseObject.data.isActive);
            } else if (responseObject.statusCode === 404) {
                showToast('Error', responseObject.responseMessage, 'error');
                $('#bank_tag_modal').modal('hide');
            }
        },
        error: function () {
            showToast('Error', 'An error occurred while fetching bank tag details.', 'error');
            $('#bank_tag_modal').modal('hide');
        },
        complete: function () {
            $('#remove_spin').addClass('d-none');
        }
    });
}

function onRemoveClicked(bankTagId) {
    $('#remove_modal').modal('show');
    $('#BankTagId').val(bankTagId);
}

function onRemoveConfirmed() {
    $('#remove_spin').removeClass('d-none');
    let _bankTagId = $('#BankTagId').val();
    $('#remove_modal').modal('hide');
    $.ajax({
        url: '/api/Banks/RemoveBankTag?id=' + _bankTagId,
        type: 'DELETE',
        async: false,
    }).always(function (responseObject) {
        if (responseObject.statusCode == 200) {
            GetBankTagList();
            showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
        }
        if (responseObject.statusCode == 404 || responseObject.statusCode == 500) {
            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
        }
        $('#remove_spin').addClass('d-none');
    });
}
