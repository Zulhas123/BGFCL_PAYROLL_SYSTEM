
$(document).ready(function () {

    loadInitialData();
    $('#save_button').on('click', function () {
        let _monthId = getMonthId();
        let dataList = [];

        let _rows = $('#ml_list tbody tr');
        $.each(_rows, function (key, item) {
            let _obj = {};
            _obj.jobCode = item.cells[1].innerHTML;
            _obj.monthId = _monthId;
            _obj.amount = parseFloat(item.cells[3].innerHTML);
            _obj.loanName = item.cells[4].innerHTML;
            _obj.employeeType = 0;

            dataList.push(_obj);
        });
        if (dataList.length > 0) {
            $.ajax({
                url: '/api/SalarySettings/StoreMonthlyLoan',
                type: 'POST',
                async: false,
                contentType: 'application/json',
                data: JSON.stringify(dataList),
                success: function (responseObject) {
                    if (responseObject.statusCode == 200) {
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

function getMonthId() {
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
            let currentMonth = new Date().getMonth()+1;
            $('#months').empty();
            $.each(months, function (key, item) {
                if (currentMonth == parseInt(item.monthId)) {
                    $('#months').append(`<option value=${item.monthId} selected>${item.monthName}</option>`);
                }
                else {
                    $('#months').append(`<option value=${item.monthId}>${item.monthName}</option>`);
                }
                
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
