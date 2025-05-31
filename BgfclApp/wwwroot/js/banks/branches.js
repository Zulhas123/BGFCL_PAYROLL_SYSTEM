let banks = [];
let _dataTable = undefined;
$(document).ready(function () {
    let pageLength = parseInt($('#page_length_select').val());

    loadInitialData();
    $('#add_button').on('click', function () {
        $('#branch_modal').modal('show');
        $('#BranchId').val('');
        $('#operation_type').val('create');
        $('#submit_button').html('Create');
        $('#branchModalLabel').html('Add Branch');
    });
    getBranchist(pageLength);

    $('#submit_button').on('click', function () {
        let branchId = $('#BranchId').val();
        let branchName = $('#BranchName').val();
        let routingNumber = $('#RoutingNumber').val();
        let bankId = $('#BankId').val();
        let address = $('#Address').val();

        if (branchId == '') {
            branchId = 0;
        }


        let dataObj = {
            id: branchId,
            userId: 0,
            schoolId: 0,
            roleId: 0,
            guestPkId: 0,
            branchName: branchName,
            routingNumber: routingNumber,
            bankId: bankId,
            address: address
        };

        let operationType = $('#operation_type').val();

        if (operationType == 'create') {


            $.ajax({
                url: '/api/Banks/CreateBranch',
                type: 'POST',
                async: false,
                data: dataObj
            }).always(function (responseObject) {
                $('.error-item').empty();
                if (responseObject.statusCode == 201) {
                    $('#BranchName').val('');
                    $('#RoutingNumber').val('');
                    $('#Address').val('');
                    $('#branchForm')[0].reset();
                    $('#branch_modal').modal('hide');
                    showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    loadInitialData();
                    getBranchist();
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
                url: '/api/Banks/UpdateBranch',
                type: 'PUT',
                async: false,
                data: dataObj
            }).always(function (responseObject) {
                $('.error-item').empty();
                if (responseObject.statusCode == 201) {
                    $('#BranchName').val('');
                    $('#RoutingNumber').val('');
                    $('#Address').val('');
                    loadInitialData();
                    $('#branchForm')[0].reset();
                    $('#branch_modal').modal('hide');
                    showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    getBranchist();
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
}


function getBranchist(pageLength) {

    if (_dataTable != undefined) {
        _dataTable.destroy();
    }
    _dataTable = new DataTable('#branch_list_table', {
        pageLength: pageLength,
       
        ajax: {
            url: '/api/Banks/GetBranches',
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
            { data: 'branchName', orderable: false },
            {
                data: 'bankName', orderable: false
            },
            { data: 'routingNumber', className: 'text-center', orderable: false },
            { data: 'address', className: 'text-center', orderable: false },
            {
                data: 'id', orderable: false,
                className: 'text-center align-middle',
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
            $('#branch_list_table thead th').find('.column-filter, br').remove();

            // Add column filters dynamically
            $('#branch_list_table thead th').each(function (index) {
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

function onEditClicked(branchId) {
    // Show the modal for editing
    $('#branch_modal').modal('show');

    // Set the operation type to 'edit' and update the button text
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#branchModalLabel').html('Edit Branch');

    // Show a loading indicator or spinner if needed
    $('#remove_spin').removeClass('d-none');

    // Fetch the branch data based on the branchId
    $.ajax({
        url: '/api/Banks/GetBranchById?id=' + branchId,
        type: 'GET',
        async: false, 
    }).always(function (responseObject) {
        if (responseObject.statusCode == 200) {
            $('#BranchId').val(responseObject.data.id); 
            $('#BranchName').val(responseObject.data.branchName);
            $('#RoutingNumber').val(responseObject.data.routingNumber);
            $('#Address').val(responseObject.data.address);
            $('#BankId').empty();
            $('#BankId').append('<option value="0">select one</option>');
            $.each(banks, function (key, item) {
                if (item.id == responseObject.data.bankId) {
                    $('#BankId').append(`<option value=${item.id} selected>${item.bankName}</option>`);
                } else {
                    $('#BankId').append(`<option value=${item.id}>${item.bankName}</option>`);
                }
            });

        } else if (responseObject.statusCode == 404) {
            showToast('Error', responseObject.responseMessage, 'error');
        }

        // Hide the loading indicator
        $('#remove_spin').addClass('d-none');
    });
}


function onRemoveClicked(branchId) {
    $('#remove_modal').modal('show');
    $('#BranchId').val(branchId);
}

function onRemoveConfirmed() {
    $('#remove_spin').removeClass('d-none');
    let _branchId = $('#BranchId').val();
    $('#remove_modal').modal('hide');
    $.ajax({
        url: '/api/Banks/RemoveBranch?id=' + _branchId,
        type: 'DELETE',
        async: false,
    }).always(function (responseObject) {
        if (responseObject.statusCode == 200) {
            getBranchist();
            $('#BranchName').val('');
            $('#RoutingNumber').val('');
            loadInitialData();
            showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
        }
        if (responseObject.statusCode == 404 || responseObject.statusCode == 500) {
            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
        }
        $('#remove_spin').addClass('d-none');
    });
}


