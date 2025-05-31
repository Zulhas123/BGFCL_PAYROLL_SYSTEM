var jss;
var updatedRow = [];
$(document).ready(function () {
    loadInitialData();

    $('#months').on('change', function () {
        $('#bonus_form').css('display', 'none');
        let monthId = getMonthId();
        $.ajax({
            url: '/api/Bonus/GetBonusByMonthId?monthId=' + monthId,
            type: 'Get',
            async: false,
            dataType: 'json',
            success: function (responseObject) {
                let bonus = responseObject.data;
                $('#BonusId').empty();
                $('#BonusId').append(`<option value='0'>Select Bonus</option>`);
                $.each(bonus, function (key, item) {
                    $('#BonusId').append(`<option value=${item.Id}>${item.bonusTitle}</option>`);

                });
            },
            error: function (responseObject) {
            }
        });

    });

    $('#years').on('change', function () {
        $('#bonus_form').css('display', 'none');
        let monthId = getMonthId();
        $.ajax({
            url: '/api/Bonus/GetBonusByMonthId?monthId=' + monthId,
            type: 'Get',
            async: false,
            dataType: 'json',
            success: function (responseObject) {
                let bonus = responseObject.data;
                $('#BonusId').empty();
                $('#BonusId').append(`<option value='0'>Select Bonus</option>`);
                $.each(bonus, function (key, item) {
                    $('#BonusId').append(`<option value=${item.id}>${item.bonusTitle}</option>`);

                });
            },
            error: function (responseObject) {
            }
        });
    });

    $('#submit_button').on('click', function () {

        if (jss != undefined) {
            jss.destroy();
            $('#jspreadsheet').empty();
        }
        debugger;
        let bonusId = $('#BonusId').val();
        let employeeTypeId = $('#EmployeeTypeId').val();

        $.ajax({
            url: `/api/Bonus/GetBonusFinalAdjustment?bonusId=${bonusId}&employeeTypeId=${employeeTypeId}`,
            type: 'Get',
            async: false,
            dataType: 'json',
            success: function (responseObject) {
                if (responseObject.statusCode == 200) {
                    $('#update_button').css('display', 'inline-block');
                    if (responseObject.data.length > 0) {
                        jss = $('#spreadsheet').jspreadsheet({
                            tableOverflow: true,
                            columnSorting: false,
                            freezeColumns: 8,
                            tableWidth: window.innerWidth - 280 + "px",
                            data: responseObject.data,
                            columns: [
                                { type: 'hidden', title: 'BonusSheetId', width: 100, name: 'bonusSheetId' },
                                { type: 'text', title: 'JobCode', width: 100, name: 'jobCode', readOnly: true, },
                                { type: 'text', title: 'employee Name', width: 100, name: 'employeeName', readOnly: true, },
                                { type: 'text', title: 'Designation', width: 100, name: 'designationName', readOnly: true, },
                                { type: 'text', title: 'Department', width: 100, name: 'departmentName', readOnly: true, },
                                { type: 'text', title: 'Account', width: 100, name: 'accountNumber', readOnly: true, },
                                { type: 'text', title: 'Bank', width: 100, name: 'bankName', readOnly: true, },
                                { type: 'text', title: 'Branch', width: 100, name: 'bankBranchName', readOnly: true, },
                                { type: 'decimal', title: 'Festival Bonus', width: 100, mask: '#,##0', name: 'festivalBonus' },
                                { type: 'decimal', title: 'Incentive Bonus', width: 100, mask: '#,##0', name: 'incentiveBonus' },
                                { type: 'decimal', title: 'Honorarium Bonus', width: 100, mask: '#,##0', name: 'honorariumBonus' },
                                { type: 'decimal', title: 'Scholarship Bonus', width: 100, mask: '#,##0', name: 'scholarshipBonus' },
                                { type: 'decimal', title: 'Deduction', width: 100, mask: '#,##0', name: 'deduction' },
                                { type: 'decimal', title: 'Revenue Stamp', width: 100, mask: '#,##0', name: 'revStamp' },
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
                                        if (parseInt(updatedRow[x].bonusSheetId) != parseInt(rowId)) {
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
                    else {
                        showToast(title = 'Error', message = 'No data found!', toastrType = 'error');
                    }

                }
                if (responseObject.statusCode == 500) {
                    $('#update_button').css('display', 'none');
                    showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                }
                if (responseObject.statusCode == 404) {
                    $('#update_button').css('display', 'none');
                    showToast(title = 'Info', message = responseObject.responseMessage, toastrType = 'info');
                }
            },
            error: function (responseObject) {
            }
        });
    });

    $('#update_button').on('click', function () {
        if (updatedRow.length > 0) {
            $.ajax({
                url: `/api/Bonus/UpdateBonusFinalAdjustment`,
                type: 'PUT',
                async: false,
                data: { bonusAdjustments: updatedRow },
                success: function (finalAdjustmentResponse) {
                    if (finalAdjustmentResponse.statusCode == 201) {
                        updatedRow = [];
                        showToast(title = 'Success', message = finalAdjustmentResponse.responseMessage, toastrType = 'success');
                    }
                    if (finalAdjustmentResponse.statusCode == 500) {
                        showToast(title = 'Error', message = finalAdjustmentResponse.responseMessage, toastrType = 'error');
                    }
                    if (finalAdjustmentResponse.statusCode == 404) {
                        showToast(title = 'Warning', message = finalAdjustmentResponse.responseMessage, toastrType = 'warning');
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
        bonusSheetId: rowData[0],
        festivalBonus: parseFloat(rowData[8]),
        incentiveBonus: parseFloat(rowData[9]),
        honorariumBonus: parseFloat(rowData[10]),
        scholarshipBonus: parseFloat(rowData[11]),
        deduction: parseFloat(rowData[12]),
        revStamp: parseFloat(rowData[13]),
 

    };
}
function getMonthId() {
    let month = $('#months').val();
    let year = $('#years').val();

    return (parseInt(year) * 100) + parseInt(month);
}
function loadInitialData() {
    $.ajax({
        url: '/api/Employees/GetEmployeeTypes',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            employeeTypes = responseObject.data;
            $('#EmployeeTypeId').empty();
            $('#EmployeeTypeId').append('<option value="0">select one</option>');
            $.each(employeeTypes, function (key, item) {
                if (item.id != 3 && item.id != 4) {
                    $('#EmployeeTypeId').append(`<option value=${item.id}>${item.employeeTypeName}</option>`);
                }

            });
        },
        error: function (responseObject) {
        }
    });

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
            $('#years').empty();
            $('#years').append(`<option value="0">Select</option>`);
            $.each(years, function (key, item) {
                $('#years').append(`<option value=${item}>${item}</option>`);
            });
        },
        error: function (responseObject) {
        }
    });
}