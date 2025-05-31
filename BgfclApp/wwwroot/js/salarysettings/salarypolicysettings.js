let salaryPolicySettings = [];
let employeeTypeFlag = '';
$(document).ready(function () {

    loadInitialData();
    $('#js_submit_button').on('click', function () {
        let operationType = $('#js_operation_type').val();
        if (operationType == 'edit') {
            let id = $('#js_id').val();
            let fuelAllow = $('#js_fuel_allow').val();
            let utilityAllow = $('#js_utility_allow').val();
            let specialBenefit = $('#js_special_benefit').val();
            let lunchRate = $('#js_lunch_rate').val();
            let tiffinRate = $('#js_tiffin_rate').val();
            let shiftAllowRate = $('#js_shift_allow_rate').val();
            let otSingle = $('#js_ot_single').val();
            let otDouble = $('#js_ot_double').val();
            let employeeClub = $('#js_employee_club').val();
            let employeeUnion = $('#js_employee_union').val();
            let welfareFund = $('#js_welfare_fund').val();
            let providentFundRate = $('#js_provident_fund').val();
            let revenueStamp = $('#js_revenue_stamp').val();
            let pensionRate = $('#js_pension_rate').val();


            let dataObj = {
                id: id,
                fuelAllow: fuelAllow,
                specialBenefit: specialBenefit,
                lunchRate: lunchRate,
                tiffinRate: tiffinRate,
                welfareFund: welfareFund,
                providentFundRate: providentFundRate,
                revenueStamp: revenueStamp,
                pensionRate: pensionRate,
                utilityAllow: utilityAllow,
                shiftAllowRate: shiftAllowRate,
                otSingle: otSingle,
                otDouble: otDouble,
                employeeClub: employeeClub,
                employeeUnion: employeeUnion,
                medical:0,

            };



            $.ajax({
                url: '/api/SalarySettings/UpdateSalaryPolicySetting',
                type: 'PUT',
                async: false,
                data: dataObj
            }).always(function (responseObject) {
                if (responseObject.statusCode == 201) {
                    $('#js_operation_type').val('');
                    showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    $('.js_input').prop('readonly', true);
                    $('#js_submit_button').html('edit');
                    $('#js_submit_button').removeClass('btn-info');
                    $('#js_submit_button').addClass('btn-primary');
                }
                if (responseObject.statusCode == 500) {
                    showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                }
            });
        }
    });
});

$('#off_submit_button').on('click', function () {

    let operationType = $('#off_operation_type').val();
    if (operationType == 'edit') {
        let id = $('#off_id').val();
        let fuelAllow = $('#off_fuel_allow').val();
        let specialBenefit = $('#off_special_benefit').val();
        let lunchRate = $('#off_lunch_rate').val();
        let tiffinRate = $('#off_tiffin_rate').val();
        let officerClub = $('#off_officer_club').val();
        let officerAssociation = $('#off_association').val();
        let welfareFund = $('#off_welfare_fund').val();
        let providentFundRate = $('#off_provident_fund').val();
        let revenueStamp = $('#off_revenue_stamp').val();
        let pensionRate = $('#off_pension_rate').val();
        let medical = $('#off_medical').val();

        let dataObj = {
            id: id,
            fuelAllow: fuelAllow,
            specialBenefit: specialBenefit,
            lunchRate: lunchRate,
            tiffinRate: tiffinRate,
            officerClub: officerClub,
            officerAssociation: officerAssociation,
            welfareFund: welfareFund,
            providentFundRate: providentFundRate,
            revenueStamp: revenueStamp,
            pensionRate: pensionRate,
            medical: medical,
        };

        $.ajax({
            url: '/api/SalarySettings/UpdateSalaryPolicySetting',
            type: 'PUT',
            async: false,
            data: dataObj
        }).always(function (responseObject) {
            if (responseObject.statusCode == 201) {
                $('#off_operation_type').val('');
                showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                $('.off_input').prop('readonly', true);
                $('#off_submit_button').html('edit');
                $('#off_submit_button').removeClass('btn-info');
                $('#off_submit_button').addClass('btn-primary');
            }
            if (responseObject.statusCode == 500) {
                showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
            }
        });
    }
});

function loadInitialData() {
    $.ajax({
        url: '/api/SalarySettings/GetSalaryPolicySettings',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            salaryPolicySettings = responseObject.data;
            $.each(salaryPolicySettings, function (key, item) {
                if (item.id == 1) {
                    $('#off_id').val(item.id);
                    $('#off_fuel_allow').val(item.fuelAllow);
                    $('#off_special_benefit').val(item.specialBenefit);
                    $('#off_lunch_rate').val(item.lunchRate);
                    $('#off_tiffin_rate').val(item.tiffinRate);
                    $('#off_officer_club').val(item.officerClub);
                    $('#off_association').val(item.officerAssociation);
                    $('#off_welfare_fund').val(item.welfareFund);
                    $('#off_provident_fund').val(item.providentFundRate);
                    $('#off_revenue_stamp').val(item.revenueStamp);
                    $('#off_pension_rate').val(item.pensionRate);
                    $('#off_medical').val(item.medical);
                }
                else {
                    $('#js_id').val(item.id);
                    $('#js_fuel_allow').val(item.fuelAllow);
                    $('#js_utility_allow').val(item.utilityAllow);
                    $('#js_special_benefit').val(item.specialBenefit);
                    $('#js_lunch_rate').val(item.lunchRate);
                    $('#js_tiffin_rate').val(item.tiffinRate);
                    $('#js_shift_allow_rate').val(item.shiftAllowRate);
                    $('#js_ot_single').val(item.otSingle);
                    $('#js_ot_double').val(item.otDouble);
                    $('#js_employee_club').val(item.employeeClub);
                    $('#js_employee_union').val(item.employeeUnion);
                    $('#js_welfare_fund').val(item.welfareFund);
                    $('#js_provident_fund').val(item.providentFundRate);
                    $('#js_revenue_stamp').val(item.revenueStamp);
                    $('#js_pension_rate').val(item.pensionRate);
                }
            });

        },
        error: function (responseObject) {
        }
    });
}

function onJsEditClick(employeeType) {
    let operationType = $('#js_operation_type').val();
    if (operationType=='') {
        $('#edit_modal').modal('show');
        employeeTypeFlag = employeeType;
    }

}

function onOffEditClick(employeeType) {
    let operationType = $('#off_operation_type').val();
    if (operationType == '') {
        $('#edit_modal').modal('show');
        employeeTypeFlag = employeeType;
    }

}

function onEditConfirmed() {
    $('#edit_modal').modal('hide');
    if (employeeTypeFlag=='js') {
        $('.js_input').prop('readonly', false);
        $('#js_operation_type').val('edit');
        $('#js_submit_button').html('submit');
        $('#js_submit_button').removeClass('btn-primary');
        $('#js_submit_button').addClass('btn-info');
    }
    if (employeeTypeFlag == 'off') {
        $('.off_input').prop('readonly', false);
        $('#off_operation_type').val('edit');
        $('#off_submit_button').html('submit');
        $('#off_submit_button').removeClass('btn-primary');
        $('#off_submit_button').addClass('btn-info');
    }
}

