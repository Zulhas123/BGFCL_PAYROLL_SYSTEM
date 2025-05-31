let bankTags = [];
let bankTypes = [];
let _dataTable = undefined;

$(document).ready(function () {
    let pageLength = parseInt($('#page_length_select').val());
    loadInitialData();

    $('#add_button').on('click', function () {
        $('#bank_type_modal').modal('show');
        $('#BankType').val('');
        $('#operation_type').val('create');
        $('#submit_button').html('Create');
        $('#banktypeModalLabel').html('Add Bank Type');
    });

    getBankTypeList(pageLength);

    $('#submit_button').on('click', function () {

        console.log("Click");
        let bankTypeId = $('#BankTypeId').val();
        let bankTypeName = $('#BankTypeName').val();
        let description = $('#Description').val();
        let isActive = $('#IsActive').is(":checked");

        if (bankTypeId === '') {
            bankTypeId = 0;
        }

        let dataObj = {
            id: bankTypeId,
            userId: 0,
            schoolId: 0,
            roleId: 0,
            guestPkId: 0,
            bankTypeName: bankTypeName,
            description: description,
            isActive: isActive
        };

        let operationType = $('#operation_type').val();
        let url = operationType === 'create' ? '/api/Banks/CreateBankType' : '/api/Banks/UpdateBankType';
        let type = operationType === 'create' ? 'POST' : 'PUT';

        $.ajax({
            url: url,
            type: type,
            async: true,
            data: dataObj
        }).always(function (responseObject) {
            $('.error-item').empty();
            if (responseObject.statusCode === 201) {
                $('#BankType').val('');
                $('#banktypeForm')[0].reset();
                $('#bank_type_modal').modal('hide');
                loadInitialData();
                showToast('Success', responseObject.responseMessage, 'success');
                getBankTypeList(pageLength);
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
        getBankTypeList(pageLength);
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

function getBankTypeList(pageLength) {
    if (_dataTable !== undefined) {
        _dataTable.destroy();
    }

    _dataTable = $('#bank_type_list_table').DataTable({
        pageLength: pageLength,
        ajax: {
            url: '/api/Banks/GetBankTypes',
            dataSrc: 'data'
        },
        columns: [
            {
                data: '',
                className: 'text-center align-middle',
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: 'bankTypeName', className: 'text-center', orderable: false },
            {
                data: 'isActive', orderable: false,
                className: 'text-center',
                render: function (data) {
                    return data ? '<span class="text-success">Active</span>' : '<span class="text-danger">Inactive</span>';
                }
            },
            {
                data: 'id',
                className: 'text-center align-middle' , orderable: false,
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
            $('#bank_type_list_table thead th').find('.column-filter, br').remove();

            $('#bank_type_list_table thead th').each(function (index) {
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

function onEditClicked(bankTypeId) {
    $('#bank_type_modal').modal('show');
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#banktypeModalLabel').html('Edit Bank Type');

    $.ajax({
        url: '/api/Banks/GetBankTypeById?id=' + bankTypeId,
        type: 'GET',
        success: function (responseObject) {
            if (responseObject.statusCode === 200) {
                $('#BankTypeId').val(responseObject.data.id);
                $('#BankTypeName').val(responseObject.data.bankTypeName);
                $('#Description').val(responseObject.data.description);
                $('#IsActive').prop('checked', responseObject.data.isActive);  
            } else {
                showToast('Error', responseObject.responseMessage, 'error');
                $('#bank_type_modal').modal('hide');
            }
        },
        error: function () {
            showToast('Error', 'An error occurred while fetching bank details.', 'error');
            $('#bank_modal').modal('hide');
        }
    });
}

function onRemoveClicked(banktypeId) {
    $('#remove_modal').modal('show');
    $('#BankTypeId').val(banktypeId);
}

function onRemoveConfirmed() {
    let _banktypeId = $('#BankTypeId').val();
    $('#remove_modal').modal('hide');

    $.ajax({
        url: '/api/Banks/RemoveBankType?id=' + _banktypeId,
        type: 'DELETE',
        async: true
    }).always(function (responseObject) {
        if (responseObject.statusCode === 200) {
            getBankTypeList(parseInt($('#page_length_select').val()));
            showToast('Success', responseObject.responseMessage, 'success');
        } else {
            showToast('Error', responseObject.responseMessage, 'error');
        }
    });
}
