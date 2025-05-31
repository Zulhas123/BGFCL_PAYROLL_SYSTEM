let _dataTable = undefined;
$(document).ready(function () {
    loadInitialData();
    $('#JobCode').select2();
    $('#JobCode').on('change', function () {
        let employeeId = $(this).find(":selected").val();
        let jobCode = $(this).find(":selected").text();
        if (employeeId.toString() == '0') {
            $('#JobCodeError').html('Select a valid Job Code');
            $('#empoyee_name').val('');
            $('#designation').val('');
            $('#department').val('');
        }
        else {
            $('#JobCodeError').empty();
            // get emplopyee info
            $.ajax({
                url: '/api/Employees/GetEmployeeViewById?id=' + employeeId,
                type: 'Get',
                async: false,
                dataType: 'json',
                success: function (responseObject) {
                    _data = responseObject.data;
                    $('#empoyee_name').val(_data.employeeName);
                    $('#designation').val(_data.designationName);
                    $('#department').val(_data.departmentName);

                },
                error: function (responseObject) {
                }
            });
        }

    });

    $('#PayModeId').on('change', function () {
        let paymode = $(this).val();
        console.log("PaymodeId", paymode);

        if (paymode == 2) {
            console.log("Paymode", paymode);

            // Clear the dropdowns
            $('#BankId').empty().append('<option value="">Select Bank</option>');
            $('#BankBranchId').empty().append('<option value="">Select Branch</option>');
            $("#AccountNumber").val("");
            // Disable the dropdowns and input field
            $('#BankId').prop('disabled', true);
            $('#BankBranchId').prop('disabled', true);
            $('#AccountNumber').prop('readonly', true); 
        } else {
           
            $('#BankId').prop('disabled', false);
            $('#BankBranchId').prop('disabled', false);
            $('#AccountNumber').prop('readonly', false);
            $.ajax({
                url: '/api/Banks/GetBanks',
                type: 'Get',
                async: false,
                dataType: 'json',
                success: function (responseObject) {
                    banks = responseObject.data;
                    $('#BankId').empty();
                    $('#BankBranchId').empty();
                    $('#BankId').append(`<option value='0'>select one</option>`);
                    $.each(banks, function (key, item) {
                        $('#BankId').append(`<option value=${item.id}>${item.bankName}</option>`);
                    });

                },
                error: function (responseObject) {
                }
            });
        }
    });



    $('#BankId').on('change', function () {
        let bankId = $(this).val();
        $('#BankBranchId').empty();

        $.ajax({
            url: '/api/Banks/GetBranchesByBankId?bankId=' + bankId,
            type: 'Get',
            async: false,
            dataType: 'json',
            success: function (responseObject) {
                branches = responseObject.data;
                $('#BankBranchId').append(`<option value='0'>select one</option>`);
                $.each(branches, function (key, item) {
                    $('#BankBranchId').append(`<option value=${item.id}>${item.branchName}</option>`);
                });

            },
            error: function (responseObject) {
            }
        });
    });

    $('#submit_button').on('click', function () {
        let _operationType = $('#operation_type').val();
        let jobCode = $('#JobCode').find(":selected").text();

        let wageGMSCB = $('#WageGMSCB').val();
        wageGMSCB = wageGMSCB == '' ? 0 : wageGMSCB;

        let houseKeepUp = $('#HouseKeepUp').val();
        houseKeepUp = houseKeepUp == '' ? 0 : houseKeepUp;

        let wESubsidy = $('#WESubsidy').val();
        wESubsidy = wESubsidy == '' ? 0 : wESubsidy;

        let fuelSubsidy = $('#FuelSubsidy').val();
        fuelSubsidy = fuelSubsidy == '' ? 0 : fuelSubsidy;


        let otherPay = $('#OtherPay').val();
        otherPay = otherPay == '' ? 0 : otherPay;

        let otherDeduction = $('#OtherDeduction').val();
        otherDeduction = otherDeduction == '' ? 0 : otherDeduction;

        let payModeId = $('#PayModeId').val();
        let bankId = $('#BankId').val();
        let bankBranchId = $('#BankBranchId').val();
        let accountNumber = $('#AccountNumber').val();

        let _object = {
            jobCode: jobCode,
            wageGMSCB: wageGMSCB,
            houseKeepUp: houseKeepUp,
            wESubsidy: wESubsidy,
            fuelSubsidy: fuelSubsidy,
            otherPay: otherPay,
            otherDeduction: otherDeduction,
            payModeId: payModeId,
            bankId: bankId,
            bankBranchId: bankBranchId,
            accountNumber: accountNumber

        };

        if (_operationType == 'edit') {
            let _rowId = $('#row_id_hidden').val();
            _object.id = _rowId;
            $.ajax({
                url: '/api/Amenities/UpdateAmenities',
                type: 'PUT',
                async: false,
                data: _object,
                success: function (responseObject) {
                    if (responseObject.statusCode == 201) {
                        $('#operation_type').val('save');
                        $('#submit_button').html('save');
                        $('.job_code_flag').css('display', 'block');
                        $('.job_code_show_flag').css('display', 'none');
                        $('.cancel_button_show').css('display', 'none');
                        loadInitialData(1);
                        showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    }
                    if (responseObject.statusCode == 500) {
                        showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                    }
                    if (responseObject.statusCode == 400) {
                        for (let error in responseObject.errors) {
                            $(`#${error}`).empty();
                            $(`#${error}`).append(responseObject.errors[error]);
                        }
                        showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                    }
                },
                error: function (responseObject) {
                }
            });
        }
        else {
            $.ajax({
                url: '/api/Amenities/CreateAmenities',
                type: 'POST',
                async: false,
                data: _object,
                success: function (responseObject) {
                    if (responseObject.statusCode == 201) {
                        loadInitialData(1);
                        showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    }
                    if (responseObject.statusCode == 500) {
                        showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                    }
                    if (responseObject.statusCode == 400) {
                        for (let error in responseObject.errors) {
                            $(`#${error}`).empty();
                            $(`#${error}`).append(responseObject.errors[error]);
                        }
                        showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                    }
                },
                error: function (responseObject) {
                }
            });
        }

    });


    $('#cancel_button').on('click', () => {
        location.reload();
    });
});

function onEditClicked(rowId) {
    $('#edit_modal').modal('show');
    $('#row_id_hidden').val(rowId);
}

function onEditConfirmed() {
    $('#remove_spin').removeClass('d-none');
    let _rowId = $('#row_id_hidden').val();
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#edit_modal').modal('hide');
    $('.job_code_flag').css('display', 'none');
    $('.job_code_show_flag').css('display', 'block');
    $('.cancel_button_show').css('display', 'inline-block');

    $.ajax({
        url: '/api/Amenities/GetAmenitiesById?amenitiesId=' + _rowId,
        type: 'GET',
        async: false,
    }).always(function (responseObject) {

        $.ajax({
            url: '/api/Employees/GetEmployeeViewByJobCode?jobcode=' + responseObject.data.jobCode,
            type: 'Get',
            async: false,
            dataType: 'json',
            success: function (employeeResponseObject) {
                _data = employeeResponseObject.data;
                $('#empoyee_name').val(_data.employeeName);
                $('#designation').val(_data.designationName);
                $('#department').val(_data.departmentName);

            },
            error: function (responseObject) {
            }
        });

        $.ajax({
            url: '/api/SalarySettings/GetPayModes',
            type: 'Get',
            async: false,
            dataType: 'json',
            success: function (payMOdeResponseObject) {
                $('#PayModeId').empty();
                payModes = payMOdeResponseObject.data;
                $('#PayModeId').append(`<option value='0'>select one</option>`);
                $.each(payModes, function (key, item) {
                    if (parseInt(item.id) == parseInt(responseObject.data.payModeId)) {
                        $('#PayModeId').append(`<option value=${item.id} selected>${item.payModeName}</option>`);
                    }
                    else {
                        $('#PayModeId').append(`<option value=${item.id}>${item.payModeName}</option>`);
                    }

                });

            },
            error: function (responseObject) {
            }
        });

        $.ajax({
            url: '/api/Banks/GetBanks',
            type: 'Get',
            async: false,
            dataType: 'json',
            success: function (bankResponseObject) {
                $('#BankId').empty();
                banks = bankResponseObject.data;
                $('#BankId').append(`<option value='0'>select one</option>`);
                $.each(banks, function (key, item) {
                    if (parseInt(item.id) == parseInt(responseObject.data.bankId)) {
                        $('#BankId').append(`<option value=${item.id} selected>${item.bankName}</option>`);
                    } else {
                        $('#BankId').append(`<option value=${item.id}>${item.bankName}</option>`);
                    }

                });

                $.ajax({
                    url: '/api/Banks/GetBranchesByBankId?bankId=' + responseObject.data.bankId,
                    type: 'Get',
                    async: false,
                    dataType: 'json',
                    success: function (branchResponseObject) {
                        $('#BankBranchId').empty();
                        branches = branchResponseObject.data;
                        $('#BankBranchId').append(`<option value='0'>select one</option>`);
                        $.each(branches, function (key, item) {
                            if (parseInt(item.id) == parseInt(responseObject.data.bankBranchId)) {
                                $('#BankBranchId').append(`<option value=${item.id} selected>${item.branchName}</option>`);
                            }
                            else {
                                $('#BankBranchId').append(`<option value=${item.id}>${item.branchName}</option>`);
                            }

                        });

                    },
                    error: function (responseObject) {
                    }
                });

            },
            error: function (responseObject) {
            }
        });

        $('#job_code_show').val(responseObject.data.jobCode);
        $('#AccountNumber').val(responseObject.data.accountNumber);

        $('#WageGMSCB').val(responseObject.data.wageGMSCB);
        $('#HouseKeepUp').val(responseObject.data.houseKeepUp);
        $('#WESubsidy').val(responseObject.data.weSubsidy);
        $('#FuelSubsidy').val(responseObject.data.fuelSubsidy);
        $('#OtherPay').val(responseObject.data.otherPay);
        $('#OtherDeduction').val(responseObject.data.otherDeduction);

    });
}
function loadInitialData(refreshFields = 0) {

    $.ajax({
        url: '/api/Amenities/GetEmployeesForAmenities?employeeTypeId=1',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            employees = responseObject.data;
            $('#JobCode').empty();
            $('#JobCode').append(`<option value='0'>select one</option>`);
            $.each(employees, function (key, item) {
                $('#JobCode').append(`<option value=${item.id}>${item.jobCode}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });

    $.ajax({
        url: '/api/SalarySettings/GetPayModes',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            payModes = responseObject.data;
            $('#PayModeId').empty();
            $('#PayModeId').append(`<option value='0'>select one</option>`);
            $.each(payModes, function (key, item) {
                $('#PayModeId').append(`<option value=${item.id}>${item.payModeName}</option>`);
            });

        },
        error: function (responseObject) {
        }
    });

    $.ajax({
        url: '/api/Banks/GetBanks',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            banks = responseObject.data;
            $('#BankId').empty();
            $('#BankBranchId').empty();
            $('#BankId').append(`<option value='0'>select one</option>`);
            $.each(banks, function (key, item) {
                $('#BankId').append(`<option value=${item.id}>${item.bankName}</option>`);
            });

        },
        error: function (responseObject) {
        }
    });


    if (_dataTable != undefined) {
        _dataTable.destroy();
    }
    _dataTable = $('#amenities_list_table').DataTable({
        ajax: {
            url: '/api/Amenities/GetAmenities',
            dataSrc: 'data'
        },
        columns: [
            {
                data: 'id',
                render: (data, type, row) => {
                    return `<button type="button" class="btn btn-primary btn-sm" onclick="onEditClicked('${data}')">edit</button>`;
                }
            },
            { data: 'jobCode' },
            { data: 'employeeName' },
            { data: 'designationName' },
            { data: 'departmentName' }
        ]
    });

    if (refreshFields == 1) {
        $('#empoyee_name').val('');
        $('#designation').val('');
        $('#department').val('');
        $('#job_code_show').val('');
        $('#AccountNumber').val('');
        $('#WageGMSCB').val('');
        $('#HouseKeepUp').val('');
        $('#WESubsidy').val('');
        $('#FuelSubsidy').val('');
        $('#OtherPay').val('');
        $('#OtherDeduction').val('');
    }
}