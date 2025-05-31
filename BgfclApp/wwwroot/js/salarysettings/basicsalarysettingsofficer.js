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


    $('#basic_salary').on('change', function () {
        let basic_salary = $(this).val();
        basic_salary = basic_salary == "" ? 0 : parseFloat(basic_salary);

        let h_allow_rate = $('#h_allow_rate').val();
        h_allow_rate = h_allow_rate == "" ? 0 : parseFloat(h_allow_rate);

        let h_allow = (h_allow_rate / 100) * basic_salary;

        $('#h_allow').val(h_allow.toFixed(2));

    });
    $('#h_allow_rate').on('change', function () {
        let basic_salary = $('#basic_salary').val();
        basic_salary = basic_salary == "" ? 0 : parseFloat(basic_salary);

        let h_allow_rate = $(this).val();
        h_allow_rate = h_allow_rate == "" ? 0 : parseFloat(h_allow_rate);

        let h_allow = (h_allow_rate / 100) * basic_salary;

        $('#h_allow').val(h_allow.toFixed(2));
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

        let payModeId = $('#pay_mode').val();
        let bankId = $('#bank').val();
        let branchId = $('#branch').val();
        let account_number = $('#account_number').val();

        let _object = {
            jobCode: jobCode,
            basicSalary: parseFloat(basic_salary),
            houseRentAllow: parseFloat(h_allow),
            familyMedicalAllow: parseFloat(medical_allow),
            personalPay: parseFloat(electricity_allow),
            likeBasic: parseFloat(gas_allow),
            conveyanceAllow: parseFloat(conveyance_allow),
            educationAllow: parseFloat(education_allow),
            washAllow: parseFloat(shifting_allow),
            chargeAllow: parseFloat(mobile_allow),
            dAidAllow: parseFloat(class_teacher_allow),
            deputationAllow: parseFloat(arrear_allow),
            officerPF: parseFloat(special_benefits),
            otherSalary: parseFloat(other_pay),
            fieldRiskAllow: parseFloat(provident_fund),
            fuelReturn: parseFloat(revenue_stamp),
            dormitoryDeduction: parseFloat(other_deduction),


            payModeId: payModeId,
            bankId: bankId,
            bankBranchId: branchId,
            accountNumber: account_number,
            
            // not need for this project.
            houseRentAllowRate: 0,
            houseRentReturn: 0,
            isMemberPF: false,
            isMemberWF: false,
            isMemberCOS: false,
            isMemberMedical: false,
            isMemberOffClub: false,
            isMemberOffAsso: false,
            monthlyTaxDeduction: 0,
            cme:0
        };

        if (_operationType == 'edit') {
            jobCode = $('#job_code_hidden').val();
            _object.jobCode = jobCode;

            $.ajax({
                url: '/api/SalarySettings/UpdateSalarySettingsOfficer',
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
                url: '/api/SalarySettings/CreateSalarySettingsOfficer',
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

    $('#pay_scale').on('change', function () {
        let gradeId = $(this).val();
        $('#basic_salary').empty();
        $.ajax({
            url: '/api/Grades/GetBasicsByGradeId?gradeId=' + gradeId,
            type: 'Get',
            async: false,
            dataType: 'json',
            success: function (responseObject) {
                basics = responseObject.data;
                $('#basic_salary').append(`<option value='0'>select one</option>`);
                $.each(basics, function (key, item) {
                    $('#basic_salary').append(`<option value=${item.basicAmount}>${item.basicAmount}</option>`);
                });

            },
            error: function (responseObject) {
            }
        });
    });

    $('#basic_salary').on('change', function () {
        let basicAmount = $(this).val();
        //let specialBenefit = (basicAmount * (5 / 100));
        let specialBenefit = Math.ceil(basicAmount * (5 / 100));
        let pf = Math.ceil(basicAmount * (10 / 100));
        $('#special_benefits').val(specialBenefit);
        $('#provident_fund').val(pf);

        let houseRentAllow = 0;
        if (basicAmount<=9700) {
            houseRentAllow = Math.ceil(basicAmount * (50 / 100));
            if (houseRentAllow < 4500) {
                houseRentAllow = 4500;
            }
        }
        if (basicAmount <= 9700 && basicAmount >=16000) {
            houseRentAllow = Math.ceil(basicAmount * (45 / 100));
            if (houseRentAllow < 4800) {
                houseRentAllow = 4800;
            }
        }
        if (basicAmount <= 16001 && basicAmount >= 35500) {
            houseRentAllow = Math.ceil(basicAmount * (40 / 100));
            if (houseRentAllow < 7000) {
                houseRentAllow = 7000;
            }
        }
        if (basicAmount >= 35501) {
            houseRentAllow = Math.ceil(basicAmount * (35 / 100));
            if (houseRentAllow < 13800) {
                houseRentAllow = 13800;
            }
        }

        $('#h_allow').val(houseRentAllow.toFixed(2));
    });
});

function loadInitialData(refreshFields = 0) {

    $.ajax({
        url: '/api/SalarySettings/GetEmployeesForSalarySetting?employeeTypeId=1',
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
            const $dropdown = $('#pay_mode');

            $dropdown.empty();
            $dropdown
                .addClass('small-dropdown') // Apply custom small-height styling
                .append(`<option value='0'>select one</option>`);

            $.each(payModes, function (key, item) {
                $dropdown.append(`<option value=${item.id}>${item.payModeName}</option>`);
            });
        }
    });

    $.ajax({
        url: '/api/Banks/GetBanks',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            payModes = responseObject.data;
            $('#bank').empty();
            $('#branch').empty();
            $('#bank').append(`<option value='0'>select one</option>`);
            $.each(payModes, function (key, item) {
                $('#bank').append(`<option value=${item.id}>${item.bankName}</option>`);
            });

        },
        error: function (responseObject) {
        }
    });

    $.ajax({
        url: '/api/Grades/GetGrades',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            grades = responseObject.data;
            $('#pay_scale').empty();
            $('#pay_scale').append(`<option value='0'>select one</option>`);
            $.each(grades, function (key, item) {
                $('#pay_scale').append(`<option value=${item.id}>${item.gradeName}</option>`);
            });

        },
        error: function (responseObject) {
        }
    });

    if (_dataTable != undefined) {
        _dataTable.destroy();
    }
    _dataTable = $('#basic_salary_setting_off_list_table').DataTable({
        ajax: {
            url: '/api/SalarySettings/GetSalaryettingsOfficers',
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
                data: 'jobCode', orderable: false,
                title: 'Job Code',
                className: 'text-center'
            },
            {
                data: 'employeeName', orderable: false,
                title: 'Employee Name',
                className: 'text-start' // LEFT ALIGNMENT
            },
            {
                data: 'designationName', orderable: false,
                title: 'Designation',
                className: 'text-center'
            },
            {
                data: 'departmentName', orderable: false,
                title: 'Department',
                className: 'text-center'
            }
        ],
        pageLength: 20,
        lengthMenu: [[10, 20, 50, 100, -1], [10, 20, 50, 100, 'All']],
        lengthChange: true,
        dom: '<"d-flex justify-content-between align-items-center mb-2"lfB>t<"d-flex justify-content-between align-items-center mt-2"ip>',
        buttons: ['copy', 'excel', 'pdf', 'print'],        
        autoWidth: false,
        language: {
            lengthMenu: 'Data per page _MENU_ '
        },
        columnDefs: [
            { targets: 0, width: '80px' },
            { targets: 1, width: '100px' },
            { targets: 2, width: '150px' },
            { targets: 3, width: '150px' },
            { targets: 4, width: '150px' }
        ],
        initComplete: function () {
            let api = this.api();

            // Add column filters
            $('#basic_salary_setting_off_list_table thead th').each(function (index) {
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
        $('#basic_salary').empty();
        $('#h_allow').val('');
        $('#medical_allow').val('');
        $('#electricity_allow').val('');
        $('#gas_allow').val('');
        $('#conveyance_allow').val('');
        $('#education_allow').val('');
        $('#shifting_allow').val('');
        $('#mobile_allow').val('');
        $('#class_teacher_allow').val('');
        $('#arrear_allow').val('');
        $('#special_benefits').val('');
        $('#other_pay').val('');
        $('#provident_fund').val('');
        $('#other_deduction').val('');

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
        url: '/api/SalarySettings/GetSalaryettingsOfficersByJobCode?jobCode=' + _jobCode,
        type: 'GET',
        async: false,
    }).always(function (responseObject) {
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

        let basic = null;

        $.ajax({
            url: '/api/Grades/GetBasics',
            type: 'Get',
            async: false,
            dataType: 'json',
            success: function (basicResponseObject) {
                let basics = basicResponseObject.data;
                
                let basicAmount = responseObject.data.basicSalary;
                for (let x = 0; x < basics.length; x++) {
                    if (parseFloat(basicAmount) == parseFloat(basics[x].basicAmount)) {
                        basic = basics[x];
                    }
                }
      

            },
            error: function (responseObject) {
            }
        });

        if (basic !=null) {
            $.ajax({
                url: '/api/Grades/GetGrades',
                type: 'Get',
                async: false,
                dataType: 'json',
                success: function (responseObject) {
                    grades = responseObject.data;
                    $('#pay_scale').empty();
                    $('#pay_scale').append(`<option value='0'>select one</option>`);
                    $.each(grades, function (key, item) {
                        if (item.id == basic.gradeId) {
                            $('#pay_scale').append(`<option value=${item.id} selected>${item.gradeName}</option>`);
                        }
                        else {
                            $('#pay_scale').append(`<option value=${item.id}>${item.gradeName}</option>`);
                        }

                    });

                },
                error: function (responseObject) {
                }
            });

            $('#basic_salary').empty();
            $.ajax({
                url: '/api/Grades/GetBasicsByGradeId?gradeId=' + basic.gradeId,
                type: 'Get',
                async: false,
                dataType: 'json',
                success: function (basicResponseObject) {
                    let basics = basicResponseObject.data;
                    $('#basic_salary').append(`<option value='0'>select one</option>`);
                    $.each(basics, function (key, item) {
                        if (item.id == basic.id) {
                            $('#basic_salary').append(`<option value=${item.basicAmount} selected>${item.basicAmount}</option>`);
                        }
                        else {
                            $('#basic_salary').append(`<option value=${item.basicAmount}>${item.basicAmount}</option>`);
                        }

                    });

                },
                error: function (responseObject) {
                }
            });
        }



        
        $('#h_allow').val(responseObject.data.houseRentAllow);
        $('#medical_allow').val(responseObject.data.familyMedicalAllow);
        $('#electricity_allow').val(responseObject.data.personalPay);
        $('#gas_allow').val(responseObject.data.likeBasic);
        $('#conveyance_allow').val(responseObject.data.conveyanceAllow);
        $('#education_allow').val(responseObject.data.educationAllow);
        $('#shifting_allow').val(responseObject.data.washAllow);
        $('#mobile_allow').val(responseObject.data.chargeAllow);
        $('#other_pay').val(responseObject.data.otherSalary);
        $('#provident_fund').val(responseObject.data.fieldRiskAllow);
        $('#other_deduction').val(responseObject.data.dormitoryDeduction);
        $('#class_teacher_allow').val(responseObject.data.dAidAllow);
        $('#special_benefits').val(responseObject.data.officerPF);
        $('#arrear_allow').val(responseObject.data.deputationAllow);
    });
}

