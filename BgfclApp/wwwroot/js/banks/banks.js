let bankTags = [];
let bankTypes = [];
let _dataTable = undefined;

$(document).ready(function () {
    let pageLength = parseInt($('#page_length_select').val());
    loadInitialData();

    $('#add_button').on('click', function () {
        $('#bank_modal').modal('show');
        $('#BankId').val('');
        $('#operation_type').val('create');
        $('#submit_button').html('Create');
        $('#bankModalLabel').html('Add Bank');
    });

    getBankList(pageLength);

    $('#submit_button').on('click', function () {
        let bankId = $('#BankId').val();
        let bankName = $('#BankName').val();
        let bankTypeId = $('#BankTypeId').val();
        let isActive = $('#IsActive').is(":checked");

        if (bankId === '') {
            bankId = 0;
        }

        let dataObj = {
            id: bankId,
            userId: 0,
            schoolId: 0,
            roleId: 0,
            guestPkId: 0,
            bankName: bankName,
            bankTagId: 0,
            bankTypeId: bankTypeId,
            isActive: isActive
        };

        let operationType = $('#operation_type').val();
        let url = operationType === 'create' ? '/api/Banks/CreateBank' : '/api/Banks/UpdateBank';
        let type = operationType === 'create' ? 'POST' : 'PUT';

        $.ajax({
            url: url,
            type: type,
            async: true,
            data: dataObj
        }).always(function (responseObject) {
            $('.error-item').empty();
            if (responseObject.statusCode === 201) {
                $('#BankName').val('');
                $('#bankForm')[0].reset();
                $('#bank_modal').modal('hide');
                loadInitialData();
                showToast('Success', responseObject.responseMessage, 'success');
                getBankList(pageLength);
                if (operationType !== 'create') {
                    $('#operation_type').val('create');
                    $('#submit_button').html('Create');
                }
            } else if (responseObject.statusCode === 400 || responseObject.statusCode === 409) {
                for (let error in responseObject.errors) {
                    $(`#${error}`).empty();
                    $(`#${error}`).append(responseObject.errors[error]);
                }
                showToast('Error', responseObject.responseMessage, 'error');
            } else if (responseObject.statusCode === 500) {
                showToast('Error', responseObject.responseMessage, 'error');
            }
        });
    });

    $('#cancel_button').on('click', () => {
        location.reload();
    });

    $('#page_length_select').on('change', function () {
        pageLength = parseInt($(this).val());
        getBankList(pageLength);
    });
});

function loadInitialData() {
    $.ajax({
        url: '/api/Banks/GetBankTypes',
        type: 'GET',
        async: true,
        dataType: 'json',
        success: function (responseObject) {
            bankTypes = responseObject.data;
            $('#BankTypeId').empty().append('<option value="0">select one</option>');
            $.each(bankTypes, function (key, item) {
                $('#BankTypeId').append(`<option value=${item.id}>${item.bankTypeName}</option>`);
            });
        }
    });
}

function getBankList(pageLength) {
    if (_dataTable !== undefined) {
        _dataTable.destroy();
    }

    _dataTable = $('#bank_list_table').DataTable({
        pageLength: pageLength,
        ajax: {
            url: '/api/Banks/GetBanks',
            dataSrc: 'data'
        },
        columns: [
            {
                data: '',
                className: 'text-center align-middle',
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                },
            },
            { data: 'bankName', orderable: false },
            { data: 'bankTypeName', orderable: false },
            {
                data: 'isActive',
                className: 'text-center', orderable: false ,
                render: function (data) {
                    return data ? '<span class="text-success">Active</span>' : '<span class="text-danger">Inactive</span>';
                }
            },
            {
                data: 'id',
                className: 'text-center align-middle', orderable: false ,
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
            $('#bank_list_table thead th').find('.column-filter, br').remove();

            $('#bank_list_table thead th').each(function (index) {
                let title = $(this).text();
                if (index !== 0 && title && title !== 'Action') {
                    $(this).append(`<br><input type="text" class="form-control form-control-sm column-filter" placeholder="Search" />`);
                }
            });

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

function onEditClicked(bankId) {
    $('#bank_modal').modal('show');
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#bankModalLabel').html('Edit Bank');

    $.ajax({
        url: '/api/Banks/GetBankById?id=' + bankId,
        type: 'GET',
        success: function (responseObject) {
            if (responseObject.statusCode === 200) {
                $('#BankId').val(responseObject.data.id);
                $('#BankName').val(responseObject.data.bankName);
                $('#IsActive').prop('checked', responseObject.data.isActive);
                $('#BankTypeId').empty().append('<option value="0">select one</option>');
                $.each(bankTypes, function (key, item) {
                    let selected = (item.id == responseObject.data.bankTypeId) ? 'selected' : '';
                    $('#BankTypeId').append(`<option value=${item.id} ${selected}>${item.bankTypeName}</option>`);
                });
            } else {
                showToast('Error', responseObject.responseMessage, 'error');
                $('#bank_modal').modal('hide');
            }
        },
        error: function () {
            showToast('Error', 'An error occurred while fetching bank details.', 'error');
            $('#bank_modal').modal('hide');
        }
    });
}

function onRemoveClicked(bankId) {
    $('#remove_modal').modal('show');
    $('#BankId').val(bankId);
}

function onRemoveConfirmed() {
    let _bankId = $('#BankId').val();
    $('#remove_modal').modal('hide');

    $.ajax({
        url: '/api/Banks/RemoveBank?id=' + _bankId,
        type: 'DELETE',
        async: true
    }).always(function (responseObject) {
        if (responseObject.statusCode === 200) {
            getBankList(parseInt($('#page_length_select').val()));
            showToast('Success', responseObject.responseMessage, 'success');
        } else {
            showToast('Error', responseObject.responseMessage, 'error');
        }
    });
}
