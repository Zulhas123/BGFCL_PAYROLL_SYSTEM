
$(document).ready(function () {

    loadInitialData();
    $('#process_button').on('click', function () {
        let month = $('#months').val();
        let year = $('#years').val();
        let _loanId = $('#LoanId').val();


        $.ajax({
            url: `/api/Loans/LoanProcess?month=${month}&year=${year}&loanId=${_loanId}`,
            type: 'Get',
            async: false,
            dataType: 'json',
            success: function (responseObject) {
                if (responseObject.statusCode == 201) {
                    showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                }
                if (responseObject.statusCode == 500) {
                    showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                }
            },
            error: function (responseObject) {
            }
        });
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
            let currentMonth = new Date().getMonth() + 1;
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

    $.ajax({
        url: '/api/Loans/GetLoans?loanType=external',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            employeeTypes = responseObject.data;
            $('#LoanId').empty();
            $('#LoanId').append('<option value="0">select one</option>');
            $.each(employeeTypes, function (key, item) {
                $('#LoanId').append(`<option value=${item.id}>${item.loanName}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });

}
