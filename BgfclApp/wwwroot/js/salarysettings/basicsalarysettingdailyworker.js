let employees = [];
let _dataTable = undefined;
$(document).ready(function () {
    $('#cancel_button').on('click', function () {
        const confirmed = confirm("Are you sure? All the edit form data will be refreshed.");
        if (confirmed) {
            location.reload();
        }
    });
    $('#revenue_stamp').val(10);
    loadInitialData();

    $('#bank').select2();
    $('#branch').select2();

    $('#job_code').on('change', function () {
        let employeeId = $(this).find(":selected").val();
        let jobCode = $(this).find(":selected").text();
        if (employeeId.toString() == '0') {
            $('#job_code_error').html('Select a valid Job Code');
            $('#empoyee_name').val('');
            $('#designation').val('');
            $('#department').val('');
        }
        else {
            $('#job_code_error').empty();
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
            // get salary settings for officer
            $.ajax({
                url: '/api/SalarySettings/GetSalarySettingsOfficerByJobCode?jobcode=' + jobCode,
                type: 'Get',
                async: false,
                dataType: 'json',
                success: function (responseObject) {
                    _data = responseObject.data;
                    console.log(_data);

                },
                error: function (responseObject) {
                }
            });
        }

    });

    $('#submit_button').on('click', function () {


        let _operationType = $('#operation_type').val();

        let jobCode = $('#job_code').find(":selected").text();

        let basic_salary = $('#basic_salary').val();
        basic_salary = basic_salary == '' ? 0 : basic_salary;

        let h_allow = $('#h_allow').val();
        h_allow = h_allow == '' ? 0 : h_allow;

        let medical_allow = $('#medical_allow').val();
        medical_allow = medical_allow == '' ? 0 : medical_allow;

        let electricity_allow = $('#electricity_allow').val();
        electricity_allow = electricity_allow == '' ? 0 : electricity_allow;

        let gas_allow = $('#gas_allow').val();
        gas_allow = gas_allow == '' ? 0 : gas_allow;

        let conveyance_allow = $('#conveyance_allow').val();
        conveyance_allow = conveyance_allow == '' ? 0 : conveyance_allow;

        let education_allow = $('#education_allow').val();
        education_allow = education_allow == '' ? 0 : education_allow;


        let shifting_allow = $('#shifting_allow').val();
        shifting_allow = shifting_allow == '' ? 0 : shifting_allow;

        let mobile_allow = $('#mobile_allow').val();
        mobile_allow = mobile_allow == '' ? 0 : mobile_allow;


        let class_teacher_allow = $('#class_teacher_allow').val();
        class_teacher_allow = class_teacher_allow == '' ? 0 : class_teacher_allow;

        let arrear_allow = $('#arrear_allow').val();
        arrear_allow = arrear_allow == '' ? 0 : arrear_allow;

        let special_benefits = $('#special_benefits').val();
        special_benefits = special_benefits == '' ? 0 : special_benefits;

        let other_pay = $('#other_pay').val();
        other_pay = other_pay == '' ? 0 : other_pay;

        let provident_fund = $('#provident_fund').val();
        provident_fund = provident_fund == '' ? 0 : provident_fund;

        let revenue_stamp = $('#revenue_stamp').val();
        revenue_stamp = revenue_stamp == '' ? 0 : revenue_stamp;

        let other_deduction = $('#other_deduction').val();
        other_deduction = other_deduction == '' ? 0 : other_deduction;

        let per_attendence = $('#per_attendence').val();
        per_attendence = per_attendence == '' ? 0 : per_attendence;
        let festival_bonus = $('#festival_bonus').val();
        festival_bonus = festival_bonus == '' ? 0 : festival_bonus;
        let is_daily_worker = $('#is_daily_worker').prop('checked');



        let payModeId = $('#pay_mode').val();
        let bankId = $('#bank').val();
        let branchId = $('#branch').val();
        let account_number = $('#account_number').val();

        let _object = {
            per_attendence: per_attendence,
            festival_bonus: festival_bonus,
            revenue_stamp: revenue_stamp,
            is_daily_worker: is_daily_worker,
            jobCode: jobCode,
            basicSalary: parseFloat(basic_salary),
            houseRentAllow: parseFloat(h_allow),
            familyMedicalAllow: parseFloat(medical_allow),
            personalPay: parseFloat(electricity_allow),
            utilityReturn: parseFloat(gas_allow),
            convenienceAllow: parseFloat(conveyance_allow),
            educationAllow: parseFloat(education_allow),
            fieldAllow: parseFloat(shifting_allow),
            fuelReturn: parseFloat(mobile_allow),
            classTeacherAllow: parseFloat(class_teacher_allow),
            arrearAllow: parseFloat(arrear_allow),
            specialBenefits: parseFloat(special_benefits),
            otherSalary: parseFloat(other_pay),
            providentFund: parseFloat(provident_fund),
            revenueStamp: parseFloat(revenue_stamp),
            dormitoryDeduction: parseFloat(other_deduction),

            payModeId: payModeId,
            bankId: bankId,
            bankBranchId: branchId,
            accountNumber: account_number,

            // not need for this project.
            houseRentAllowRate: 0,
            isMemberPF: false,
            isMemberWF: false,
            isMemberCOS: false,
            isMemberEmpClub: false,
            isMemberEmpUnion: false,

        };


        if (_operationType == 'edit') {
            jobCode = $('#job_code_hidden').val();
            _object.jobCode = jobCode;

            $.ajax({
                url: '/api/SalarySettings/UpdateSalarySettingsJuniorStaff',
                type: 'POST',
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
                },
                error: function (responseObject) {
                }
            });
        }
        else {
            $.ajax({
                url: '/api/SalarySettings/CreateSalarySettingsJuniorStaff',
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
                },
                error: function (responseObject) {
                }
            });
        }

    });

    $('#bank').on('change', function () {
        debugger;
        let bankId = $(this).val();
        $('#branch').empty();

        $.ajax({
            url: '/api/Banks/GetBranchesByBankId?bankId=' + bankId,
            type: 'Get',
            async: false,
            dataType: 'json',
            success: function (responseObject) {
                branches = responseObject.data;
                console.log(branches)
                $('#branch').append(`<option value='0'>select one</option>`);
                $.each(branches, function (key, item) {
                    $('#branch').append(`<option value=${item.id}>${item.branchName}</option>`);
                });

            },
            error: function (responseObject) {
            }
        });
    });
});

function loadInitialData(refreshFields = 0) {
    console.log("Salary settins")
    $.ajax({
        url: '/api/SalarySettings/GetEmployeesForSalarySetting?employeeTypeId=3',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            employees = responseObject.data;
            $('#job_code').empty();
            $('#job_code').append(`<option value='0'>select one</option>`);
            $.each(employees, function (key, item) {
                $('#job_code').append(`<option value=${item.id}>${item.jobCode}</option>`);
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
            $('#pay_mode').empty();
            $('#pay_mode').append(`<option value='0'>select one</option>`);
            $.each(payModes, function (key, item) {
                $('#pay_mode').append(`<option value=${item.id}>${item.payModeName}</option>`);
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
            $('#bank').empty();
            $('#branch').empty();
            $('#bank').append(`<option value='0'>select one</option>`);
            $.each(banks, function (key, item) {
                $('#bank').append(`<option value=${item.id}>${item.bankName}</option>`);
            });

        },
        error: function (responseObject) {
        }
    });


    if (_dataTable != undefined) {
        _dataTable.destroy();
    }
    _dataTable = $('#basic_salary_setting_worker_list_table').DataTable({
        ajax: {
            url: '/api/SalarySettings/GetSalaryettingsworker',
            dataSrc: 'data'
        },
        columns: [
            {
                data: 'jobCode',
                title: 'Action',
                render: (data, type, row) => {
                    return `<button type="button" class="btn btn-primary btn-sm" onclick="onEditClicked('${data}')"><i class="fas fa-edit"></i></button>`;
                },
                className: 'text-center'
            },
            {
                data: 'jobCode',
                title: 'Job Code',
                className: 'text-center',
                orderable: false
            },
            {
                data: 'employeeName',
                title: 'Employee Name',
                className: 'text-start',
                orderable: false
            },
            {
                data: 'designationName',
                title: 'Designation',
                className: 'text-center',
                orderable: false
            },
            {
                data: 'departmentName',
                title: 'Department',
                className: 'text-center',
                orderable: false
            }
        ],
        pageLength: 20,
        lengthMenu: [[10, 20, 50, 100, -1], [10, 20, 50, 100, 'All']],
        lengthChange: true,
        dom: '<"d-flex justify-content-between align-items-center mb-2"lfB>t<"d-flex justify-content-between align-items-center mt-2"ip>',
        buttons: ['copy', 'excel', 'pdf', 'print'],
        language: {
            lengthMenu: 'Data per page _MENU_'
        },
        autoWidth: false, // Disable auto width to use defined widths
        columnDefs: [
            { targets: 0, width: '80px' }, // Action Button
            { targets: 1, width: '100px' }, // Job Code
            { targets: 2, width: '150px' }, // Employee Name
            { targets: 3, width: '150px' }, // Designation
            { targets: 4, width: '150px' }  // Department
        ],
        initComplete: function () {
            let api = this.api();

            // Add column filters
            $('#basic_salary_setting_js_list_table thead th').each(function (index) {
                let title = $(this).text();
                if (title && title !== 'Action') {
                    $(this).html(`${title}<br><input type="text" class="form-control form-control-sm column-filter" placeholder=" " />`);
                }
            });

            // Apply column filtering
            api.columns().every(function () {
                let that = this;
                $('input', this.header()).on('keyup change', function () {
                    if (that.search() !== this.value) {
                        that.search(this.value).draw();
                    }
                });
            });
        }
    });
    // Custom page length dropdown
    $('#page_length_select').on('change', function () {
        let length = parseInt($(this).val(), 10);
        _dataTable.page.len(length).draw();
    });

    if (refreshFields == 1) {
        $('#empoyee_name').val('');
        $('#designation').val('');
        $('#department').val('');
        $('#job_code_show').val('');
        $('#account_number').val('');
        $('#basic_salary').val('');
        $('#h_allow').val('');
        $('#medical_allow').val('');
        $('#electricity_allow').val('');
        $('#gas_allow').val('');
        $('#conveyance_allow').val('');
        $('#education_allow').val('');
        $('#shifting_allow').val('');
        $('#mobile_allow').val('');
        $('#other_pay').val('');
        $('#provident_fund').val('');
        $('#other_deduction').val('');
        $('#class_teacher_allow').val('');
        $('#special_benefits').val('');
        $('#arrear_allow').val('');
    }
}

function onEditClicked(job_code) {
    $('#edit_modal').modal('show');
    $('#job_code_hidden').val(job_code);
}

function onEditConfirmed() {

    $('#remove_spin').removeClass('d-none');
    let _jobCode = $('#job_code_hidden').val();
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#edit_modal').modal('hide');
    $('.job_code_flag').css('display', 'none');
    $('.job_code_show_flag').css('display', 'block');
    $('.cancel_button_show').css('display', 'inline-block');

    $.ajax({
        url: '/api/SalarySettings/GetSalaryettingsJuniorStaffByJobCode?jobCode=' + _jobCode,
        type: 'GET',
        async: false,
    }).always(function (responseObject) {
        console.log(responseObject)
        $.ajax({
            url: '/api/Employees/GetEmployeeViewByJobCode?jobcode=' + _jobCode,
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
                $('#pay_mode').empty();
                payModes = payMOdeResponseObject.data;
                console.log(payModes, responseObject.data.payModeId)
                $('#pay_mode').append(`<option value='0'>select one</option>`);
                $.each(payModes, function (key, item) {
                    if (parseInt(item.id) == parseInt(responseObject.data.payModeId)) {
                        $('#pay_mode').append(`<option value=${item.id} selected>${item.payModeName}</option>`);
                    }
                    else {
                        $('#pay_mode').append(`<option value=${item.id}>${item.payModeName}</option>`);
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
                $('#bank').empty();
                banks = bankResponseObject.data;
                console.log(responseObject.data.bankId)
                $('#bank').append(`<option value='0'>select one</option>`);
                $.each(banks, function (key, item) {
                    if (parseInt(item.id) == parseInt(responseObject.data.bankId)) {
                        $('#bank').append(`<option value=${item.id} selected>${item.bankName}</option>`);
                    } else {
                        $('#bank').append(`<option value=${item.id}>${item.bankName}</option>`);
                    }

                });

                $.ajax({
                    url: '/api/Banks/GetBranchesByBankId?bankId=' + responseObject.data.bankId,
                    type: 'Get',
                    async: false,
                    dataType: 'json',
                    success: function (branchResponseObject) {
                        $('#branch').empty();
                        branches = branchResponseObject.data;
                        $('#branch').append(`<option value='0'>select one</option>`);

                        $.each(branches, function (key, item) {
                            if (parseInt(item.id) == parseInt(responseObject.data.bankBranchId)) {
                                $('#branch').append(`<option value=${item.id} selected>${item.branchName}</option>`);
                            }
                            else {
                                $('#branch').append(`<option value=${item.id}>${item.branchName}</option>`);
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

        $('#job_code_show').val(_jobCode);
        $('#account_number').val(responseObject.data.accountNumber);
        $('#basic_salary').val(responseObject.data.basicSalary);
        $('#h_allow').val(responseObject.data.houseRentAllow);
        $('#medical_allow').val(responseObject.data.familyMedicalAllow);
        $('#electricity_allow').val(responseObject.data.personalPay);
        $('#gas_allow').val(responseObject.data.utilityReturn);
        $('#conveyance_allow').val(responseObject.data.convenienceAllow);
        $('#education_allow').val(responseObject.data.educationAllow);
        $('#shifting_allow').val(responseObject.data.fieldAllow);
        $('#mobile_allow').val(responseObject.data.fuelReturn);
        $('#other_pay').val(responseObject.data.otherSalary);
        $('#provident_fund').val(responseObject.data.providentFund);
        $('#other_deduction').val(responseObject.data.dormitoryDeduction);
        $('#class_teacher_allow').val(responseObject.data.classTeacherAllow);
        $('#special_benefits').val(responseObject.data.specialBenefits);
        $('#arrear_allow').val(responseObject.data.arrearAllow);

        $('#per_attendence').val(responseObject.data.per_attendence);
        $('#festival_bonus').val(responseObject.data.festival_bonus);
        $('#is_daily_worker').prop('checked', responseObject.data.is_daily_worker);
        console.log("attendence", responseObject)
    });
}