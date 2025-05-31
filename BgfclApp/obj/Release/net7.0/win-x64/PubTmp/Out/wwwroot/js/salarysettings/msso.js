
$(document).ready(function () {

    loadInitialData();
    $('#save_button').on('click', function () {
        let _monthId = getMonthId();
        let dataList = [];

        console.log(_monthId);
        let _rows = $('#msso_list tbody tr');
        $.each(_rows, function (key,item) {
            let _obj = {};
            _obj.jobCode = item.cells[1].innerHTML;
            _obj.monthId = _monthId;
            _obj.arrearSalary = parseFloat(item.cells[3].innerHTML);
            _obj.workDays = parseInt(item.cells[4].innerHTML);
            _obj.advanceSalary = parseFloat(item.cells[5].innerHTML);
            _obj.otherDeduction = parseFloat(item.cells[6].innerHTML);
            _obj.specialDeduction = parseFloat(item.cells[7].innerHTML);
            _obj.advanceDeduction = parseFloat(item.cells[8].innerHTML);
            _obj.hospitalBill = parseFloat(item.cells[9].innerHTML);
            _obj.tMBill = parseFloat(item.cells[10].innerHTML);
            _obj.otherAllow = parseFloat(item.cells[11].innerHTML);

            dataList.push(_obj);
        });
        if (dataList.length > 0) {
            $.ajax({
                url: '/api/SalarySettings/StoreMonthlySalarySettingOfficer',
                type: 'POST',
                async: false,
                contentType: 'application/json',
                data: JSON.stringify(dataList),
                success: function (responseObject) {
                    if (responseObject.statusCode==200) {
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

});

function getMonthId()
{
    let month = $('#months').val();
    let year = $('#years').val();

    return (parseInt(year) * 100) + parseInt(month);
}

function loadInitialData() {

    $.ajax({
        url: '/api/Others/GetMonths',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            let months = responseObject.data;
            $('#months').empty();
            $.each(months, function (key, item) {
                $('#months').append(`<option value=${item.monthId}>${item.monthName}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });

    $.ajax({
        url: '/api/Others/GetYears',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            let years = responseObject.data;
            let currentYear = new Date().getFullYear();
            $('#years').empty();
            $.each(years, function (key, item) {
                if (item == currentYear) {
                    $('#years').append(`<option value=${item} selected>${item}</option>`);
                }
                else {
                    $('#years').append(`<option value=${item}>${item}</option>`);
                }
            });
        },
        error: function (responseObject) {
        }
    });

}
