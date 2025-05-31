var jss;
var updatedRow = [];
$(document).ready(function () {
    loadInitialData();

    $('#submit_button').on('click', function () {

        if (jss != undefined) {
            jss.destroy();
            $('#jspreadsheet').empty();
        }

        let monthId = getMonthId();

        $.ajax({
            url: `/api/Amenities/GetAmenitiesFinalAdjustmentsByMonthId?monthId=${monthId}`,
            type: 'Get',
            async: false,
            dataType: 'json',
            success: function (responseObject) {
                if (responseObject.statusCode == 200) {
                    $('#update_button').css('display', 'inline-block');
                    jss = $('#spreadsheet').jspreadsheet({
                        tableOverflow: true,
                        columnSorting: false,
                        freezeColumns: 8,
                        tableWidth: window.innerWidth - 280 + "px",
                        data: responseObject.data,
                        columns: [
                            { type: 'hidden', title: 'AmenitiesReportId', width: 100, name: 'amenitiesReportId' },
                            { type: 'text', title: 'JobCode', width: 100, name: 'jobCode', readOnly: true, },
                            { type: 'text', title: 'employee Name', width: 100, name: 'employeeName', readOnly: true, },
                            { type: 'text', title: 'Designation', width: 100, name: 'designationName', readOnly: true, },
                            { type: 'text', title: 'Department', width: 100, name: 'departmentName', readOnly: true, },
                            { type: 'text', title: 'Account', width: 100, name: 'accountNumber', readOnly: true, },
                            { type: 'text', title: 'Bank', width: 100, name: 'bankName', readOnly: true, },
                            { type: 'text', title: 'Branch', width: 100, name: 'bankBranchName', readOnly: true, },
                            { type: 'decimal', title: 'WageGMSCB', width: 100, mask: '#,##0', name: 'wageGMSCB' },
                            { type: 'decimal', title: 'House Keeping', width: 100, mask: '#,##0', name: 'houseKeepUp' },
                            { type: 'decimal', title: 'Fuel Subsidy', width: 100, mask: '#,##0', name: 'fuelSubsidy' },
                            { type: 'decimal', title: 'WE Subsidy', width: 100, mask: '#,##0', name: 'weSubsidy' },
                            { type: 'decimal', title: 'Other Deduction', width: 100, mask: '#,##0', name: 'otherDeduction' },
                            { type: 'decimal', title: 'Other Pay', width: 100, mask: '#,##0', name: 'otherPay' },
                            { type: 'decimal', title: 'Revenue Stamp', width: 100, mask: '#,##0', name: 'revenueStamp' },
                        ],
                        onchange: function (instance, cell, x, y, value) {
                            var _newArray = [];
                            var rowId = jss.getValueFromCoords(0, y);
                            var retrivedData = retrivedObject(jss.getRowData(y));
                            if (updatedRow.length == 0) {
                                updatedRow.push(retrivedData);
                            }
                            else {
                                for (let x = 0; x < updatedRow.length; x++) {
                                    if (parseInt(updatedRow[x].amenitiesReportId) != parseInt(rowId)) {
                                        _newArray.push(updatedRow[x]);
                                    }
                                }
                                _newArray.push(retrivedData);
                                updatedRow = _newArray;
                            }
                        },
                        contextMenu: function (obj, x, y, e) {
                            return [];
                        }
                    });

                }
                if (responseObject.statusCode == 404) {
                    $('#update_button').css('display', 'none');
                    showToast(title = 'Info', message = responseObject.responseMessage, toastrType = 'info');
                }
                if (responseObject.statusCode == 500) {
                    $('#update_button').css('display', 'none');
                    showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                }
            },
            error: function (responseObject) {
            }
        });
    });

    $('#update_button').on('click', function () {
        if (updatedRow.length > 0) {
            $.ajax({
                url: `/api/Amenities/UpdateAmenitiesFinalAdjustment`,
                type: 'PUT',
                contentType: 'application/json',
                dataType: 'json',
                async: false,
                data: JSON.stringify({ amenitiesFinalAdjustments: updatedRow }),
                success: function (finalAdjustmentResponse) {
                    if (finalAdjustmentResponse.statusCode == 201) {
                        updatedRow = [];
                        showToast(title = 'Success', message = finalAdjustmentResponse.responseMessage, toastrType = 'success');
                    }
                    if (finalAdjustmentResponse.statusCode == 404) {
                        showToast(title = 'Warning', message = finalAdjustmentResponse.responseMessage, toastrType = 'warning');
                    }
                    if (finalAdjustmentResponse.statusCode == 500) {
                        showToast(title = 'Error', message = finalAdjustmentResponse.responseMessage, toastrType = 'error');
                    }
                },
                error: function () {

                }

            });
        }
        else {
            showToast(title = 'Warning', message = 'No data found to update', toastrType = 'warning');
        }
    });
});

function retrivedObject(rowData) {
    return {
        amenitiesReportId: rowData[0],
        wageGMSCB: parseFloat(rowData[8]),
        houseKeepUp: parseFloat(rowData[9]),
        fuelSubsidy: parseFloat(rowData[10]),
        weSubsidy: parseFloat(rowData[11]),
        otherDeduction: parseFloat(rowData[12]),
        otherPay: parseFloat(rowData[13]),
        revenueStamp: parseFloat(rowData[14]),


    };
}
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
                if (currentMonth == item.monthId) {
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
                if (currentYear==item) {
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