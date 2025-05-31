let banks = [];
let _dataTable = undefined;
$(document).ready(function () {
    let pageLength = parseInt($('#page_length_select').val());

    loadInitialData();
    $('#add_button').on('click', function () {
        $('#account_modal').modal('show');
        $('#AccountId').val('');
        $('#operation_type').val('create');
        $('#submit_button').html('Create');
        $('#accountModalLabel').html('Add Accounts');
    });
    getAccountlist(pageLength);

    $('#submit_button').on('click', function () {
        let accountId = $('#AccountId').val();
        let bankId = $('#BankId').val();
        let branchId = $('#BranchId').val();
        let accountType = $('#AccountType').val();
        let accountNumber = $('#AccountNumber').val();
        let accountName = $('#AccountName').val();
        let notes = $('#Notes').val();

        if (accountId == '') {
            accountId = 0;
        }


        let dataObj = {
            id: accountId,
            bankId: bankId,
            branchId: branchId,
            accountType: accountType,
            accountNumber: accountNumber,
            accountName: accountName,
            notes: notes,
            openingBalance: 0,
            BankTypeId:1

        };

        let operationType = $('#operation_type').val();

        if (operationType == 'create') {


            $.ajax({
                url: '/api/Banks/CreateAccount',
                type: 'POST',
                async: false,
                data: dataObj
            }).always(function (responseObject) {
                $('.error-item').empty();
                if (responseObject.statusCode == 201) {
                    $('#BankId').val('');
                    $('#BranchId').val('');
                    $('#AccountType').val('');
                    $('#AccountNumber').val('');
                    $('#AccountName').val('');
                    $('#Notes').val('');
                    $('#accountForm')[0].reset();
                    $('#account_modal').modal('hide');
                    showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    loadInitialData();
                    getAccountlist();
                }
                if (responseObject.statusCode == 400 || responseObject.statusCode == 409) {
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
        else {

            $.ajax({
                url: '/api/Banks/UpdateAccount',
                type: 'PUT',
                async: false,
                data: dataObj
            }).always(function (responseObject) {
                $('.error-item').empty();
                if (responseObject.statusCode == 201) {
                    $('#BankId').val('');
                    $('#BranchId').val('');
                    $('#AccountType').val('');
                    $('#AccountNumber').val('');
                    $('#AccountName').val('');
                    $('#Notes').val('');
                    loadInitialData();
                    $('#accountForm')[0].reset();
                    $('#account_modal').modal('hide');
                    showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    getAccountlist();
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
        getBranchist(pageLength);
    });
});

function loadInitialData() {
    $.ajax({
        url: '/api/Banks/GetBanks',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            banks = responseObject.data;
            $('#BankId').empty();
            $('#BankId').append('<option value="0">select one</option>');
            $.each(banks, function (key, item) {
                $('#BankId').append(`<option value=${item.id}>${item.bankName}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });
    $.ajax({
        url: '/api/Banks/GetBranches',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            banks = responseObject.data;
            $('#BranchId').empty();
            $('#BranchId').append('<option value="0">select one</option>');
            $.each(banks, function (key, item) {
                $('#BranchId').append(`<option value=${item.id}>${item.branchName}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });
}


function getAccountlist(pageLength) {

    if (_dataTable != undefined) {
        _dataTable.destroy();
    }

    _dataTable = new DataTable('#accounts_list_table', {
        pageLength: pageLength,
        autoWidth: false,

        ajax: {
            url: '/api/Banks/GetAccounts',
            dataSrc: 'data'
        },
        columns: [
            {
                data: '',
                width: '5%',
                className: 'text-center align-middle',
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: 'bankName', width: '15%', orderable: false },
            { data: 'branchName', width: '15%', orderable: false },
            { data: 'accountNumber', width: '15%', className: 'text-center', orderable: false },
            { data: 'openingBalance', width: '15%', className: 'text-center', orderable: false },
            { data: 'notes', width: '25%', orderable: false },
            {
                data: 'id',
                width: '10%',
                orderable: false,
                className: 'text-center align-middle',
                render: function (data, type, row, meta) {
                    return `<button type="button" class="btn btn-primary btn-sm" onclick="onEditClicked(${data})">
                                <i class="fas fa-edit"></i>
                            </button> 
                            <button type="button" class="btn btn-danger btn-sm" onclick="onRemoveClicked(${data})">
                                <i class="fas fa-trash"></i>
                            </button>`;
                }
            }
        ],
        lengthMenu: [[10, 20, 50, 100, -1], [10, 20, 50, 100, 'All']],
        lengthChange: true,
        dom: '<"d-flex justify-content-between align-items-center mb-2"lfB>t<"d-flex justify-content-between align-items-center mt-2"ip>',
        buttons: ['copy', 'excel', 'pdf', 'print'],
        language: {
            lengthMenu: 'Data per page _MENU_  '
        },
        initComplete: function () {
            let api = this.api();
            $('#accounts_list_table thead th').find('.column-filter, br').remove();

            // Add column filters dynamically
            $('#accounts_list_table thead th').each(function (index) {
                let title = $(this).text();
                if (index !== 0 && title && title !== 'Action') {
                    $(this).append(`<br><input type="text" class="form-control form-control-sm column-filter" placeholder="Search" />`);
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




function onEditClicked(accountId) {
    // Show the modal for editing
    $('#account_modal').modal('show');

    // Set the operation type to 'edit' and update the button text
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#accountModalLabel').html('Edit Account');

    // Show a loading indicator or spinner if needed
    $('#remove_spin').removeClass('d-none');

    // Fetch the branch data based on the branchId
    $.ajax({
        url: '/api/Banks/GetAccountById?id=' + accountId,
        type: 'GET',
        async: false, 
    }).always(function (responseObject) {
        console.log("data", responseObject)
        if (responseObject.statusCode == 200) {
            $('#AccountId').val(responseObject.data.id); 
            $('#AccountType').val(responseObject.data.accountType);
            $('#AccountNumber').val(responseObject.data.accountNumber);
            $('#AccountName').val(responseObject.data.accountName);
            $('#Notes').val(responseObject.data.notes);
            $('#BankId').empty();
            $('#BankId').append('<option value="0">select one</option>');
            $.each(banks, function (key, item) {
                if (item.id == responseObject.data.bankId) {
                    $('#BankId').append(`<option value=${item.id} selected>${item.bankName}</option>`);
                } else {
                    $('#BankId').append(`<option value=${item.id}>${item.bankName}</option>`);
                }
            });
            $('#BranchId').empty();
            $('#BranchId').append('<option value="0">select one</option>');
            $.each(banks, function (key, item) {
                if (item.id == responseObject.data.branchId) {
                    $('#BranchId').append(`<option value=${item.id} selected>${item.branchName}</option>`);
                } else {
                    $('#BranchId').append(`<option value=${item.id}>${item.branchName}</option>`);
                }
            });

        } else if (responseObject.statusCode == 404) {
            showToast('Error', responseObject.responseMessage, 'error');
        }

        // Hide the loading indicator
        $('#remove_spin').addClass('d-none');
    });
}


function onRemoveClicked(accountId) {
    $('#remove_modal').modal('show');
    $('#AccountId').val(accountId);
}

function onRemoveConfirmed() {
    $('#remove_spin').removeClass('d-none');
    let _accountId = $('#AccountId').val();
    $('#remove_modal').modal('hide');
    $.ajax({
        url: '/api/Banks/RemoveAccount?id=' + _accountId,
        type: 'DELETE',
        async: false,
    }).always(function (responseObject) {
        if (responseObject.statusCode == 200) {
            getBranchist();
            $('#AccountNumber').val('');
            $('#AccountName').val('');
            loadInitialData();
            showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
        }
        if (responseObject.statusCode == 404 || responseObject.statusCode == 500) {
            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
        }
        $('#remove_spin').addClass('d-none');
    });
}


