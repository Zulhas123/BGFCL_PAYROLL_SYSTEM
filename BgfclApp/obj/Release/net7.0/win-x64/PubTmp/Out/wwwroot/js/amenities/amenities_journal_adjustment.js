var jss;
var updatedRow = [];
var newRowData = [];

$(document).ready(function () {
    loadInitialData();
   
    $('#submit_button').on('click', function () {
        if (jss !== undefined) {
            jss.destroy();
            $('#jspreadsheet').empty();
        }

        let _monthId = getMonthId();
            $.ajax({
                url: `/api/amenities/amenitiesJournalAdjustment?monthId=${_monthId}`,
                type: 'GET',
                dataType: 'json',
                success: function (responseObject) {
                    if (responseObject.statusCode === 200) {
                        $('#update_button').css('display', 'inline-block');
                        if (responseObject.data.length > 0) {
                            jss = $('#spreadsheet').jspreadsheet({
                                columnSorting: false,
                                tableWidth: window.innerWidth - 280 + "px",
                                data: responseObject.data,
                                columns: [
                                    { type: 'hidden', title: 'Id', width: 100, name: 'id' },
                                    { type: 'hidden', title: 'Month Id', width: 100, name: 'monthId' },
                                    { type: 'hidden', title: 'Journal Master Id', width: 100, name: 'journalMasterId' },
                                    { type: 'text', title: 'Account Number', width: 150, name: 'accountNumber' },
                                    { type: 'text', title: ' C.C.', width: 150, name: 'journalCode' },
                                    { type: 'text', title: 'Details', width: 200, name: 'details' },
                                    { type: 'decimal', title: 'Debit', width: 150, name: 'debit' },
                                    { type: 'decimal', title: 'Credit', width: 150, name: 'credit' },
                                    { type: 'html', title: 'Actions', width: 100, readOnly: true }
                                ],

                                onchange: function (instance, cell, x, y, value) {
                                    var rowId = jss.getValueFromCoords(0, y);
                                    var retrivedData = retrivedObject(jss.getRowData(y));
                                    var journalMasterId = responseObject.data[0].journalMasterId;

                                    retrivedData.journalMasterId = journalMasterId || null;

                                    // Check if it's a new row (rowId is null or empty)
                                    if (!rowId || rowId === "") {
                                        // Ensure the index in newRowData matches the row (y)
                                        if (!newRowData[y]) {
                                            newRowData[y] = retrivedData;  // Store new row data at the correct index
                                        } else {
                                            newRowData[y] = retrivedData;  // Update the existing new row data
                                        }
                                    } else {
                                        // Update for existing rows
                                        var existingRowIndex = updatedRow.findIndex(function (row) {
                                            return parseInt(row.id) === parseInt(rowId);
                                        });

                                        if (existingRowIndex === -1) {
                                            // Row doesn't exist yet, so add it
                                            updatedRow.push(retrivedData);
                                        } else {
                                            // Row exists, update the existing row data
                                            updatedRow[existingRowIndex] = retrivedData;
                                        }
                                    }
                                },


                                contextMenu: function (obj, x, y, e) {
                                    return []; // Disable the right-click context menu
                                }
                            });

                            // Add action buttons after initializing the spreadsheet
                            addActionButtons();

                            $('#spreadsheet').on('click', '.add-row', function () {
                                var rowIdx = $(this).closest('tr').index();

                                // Add new row to the spreadsheet
                                jss.insertRow([], rowIdx + 1);

                                addActionButtons();
                            });

                            $('#spreadsheet').on('click', '.delete-row', function () {
                                var rowIdx = $(this).closest('tr').index();
                                if (rowIdx !== null && rowIdx !== undefined && rowIdx >= 0) {
                                    var rowId = jss.getValueFromCoords(0, rowIdx);
                                    console.log("rowId:", rowId);

                                    if (rowId) {
                                        $.ajax({
                                            url: `/api/Amenities/DeleteAmenitiesjournal?id=` + rowId,
                                            contentType: 'application/json',
                                            dataType: 'json',
                                            type: 'POST',
                                            success: function (responseObject) {
                                                if (responseObject.statusCode === 200) {
                                                    jss.deleteRow(rowIdx); // Delete row only if API call succeeds
                                                    showToast('Success', responseObject.responseMessage, 'success');
                                                } else {
                                                    showToast('Error', responseObject.responseMessage, 'error');
                                                }
                                            },
                                            error: function (xhr, status, error) {
                                                console.error("API error response:", xhr.responseText, status, error);
                                                showToast('Error', 'An error occurred while deleting the row.', 'error');
                                            }
                                        });
                                    } else {
                                        console.error("Row ID is invalid or null for row index:", rowIdx);
                                    }
                                } else {
                                    console.log("Invalid row index:", rowIdx);
                                }

                                addActionButtons(); // Re-add action buttons if needed
                            });




                        } else {
                            $('#update_button').css('display', 'none');
                            showToast('Error', 'No data found!', 'error');
                        }
                    } else if (responseObject.statusCode === 500) {
                        $('#update_button').css('display', 'none');
                        showToast('Error', responseObject.responseMessage, 'error');
                    }
                },
                error: function (error) {
                    console.error('Error loading data:', error);
                    showToast('Error', 'Failed to load data.', 'error');
                }
            });
      
       
    });


    $('#update_button').on('click', function () {

        function isValidRow(row) {
            console.log("Validating row:", row);

            const hasValidAccountNumber = row.accountNumber !== undefined && row.accountNumber !== '';
            const hasValidJournalCode = !isNaN(row.journalCode) && row.journalCode !== null;
            const hasValidDetails = row.details !== undefined && row.details !== '';
            const hasValidDebit = !isNaN(row.debit) && row.debit >= 0;
            const hasValidCredit = row.credit === undefined || (!isNaN(row.credit) && row.credit >= 0);

            console.log({
                hasValidAccountNumber,
                hasValidJournalCode,
                hasValidDetails,
                hasValidDebit,
                hasValidCredit
            });

            return hasValidAccountNumber &&
                hasValidJournalCode &&
                hasValidDetails &&
                hasValidDebit &&
                hasValidCredit;
        }

        let validRowData = newRowData.filter(isValidRow);
        for (let x of validRowData) {
            x.accountNumber = x.accountNumber.toString();
            x.journalCode = x.journalCode.toString();
        }




        // Submit new rows to the API
        if (validRowData.length > 0) {
            $.ajax({
                url: `/api/amenities/InsertJournalAdjustment`,
                type: 'POST',
                async: false,
                contentType: 'application/json',
                data: JSON.stringify(validRowData),
                success: function (response) {
                    if (response.statusCode === 200) {
                        showToast('Success', 'New rows added successfully!', 'success');
                        newRowData = [];
                    } else {
                        showToast('Error', response.responseMessage, 'error');
                    }
                },
                error: function (error) {
                    console.error('Error adding new rows:', error);
                    showToast('Error', 'Failed to add new rows.', 'error');
                }
            });
        }

        // Submit updated rows to the API
        if (updatedRow.length > 0) {
            console.log("Click");
            updatedRow = updatedRow.map(function (row) {
                return {
                    ...row,
                    accountNumber: row.accountNumber.toString(),
                    journalCode: row.journalCode.toString(),
                    debit: row.debit !== null ? parseFloat(row.debit) : null, // Ensure debit is either a number or null
                    credit: row.credit !== null ? parseFloat(row.credit) : null // If there's a credit field
                };
            });

            $.ajax({
                url: `/api/Amenities/UpdateJournalAdjustment`,
                contentType: 'application/json',
                dataType: 'json',
                type: 'POST',
                async: false,
                data: JSON.stringify({ journalAdjustment: updatedRow }),
                success: function (responseObject) {
                    if (responseObject.statusCode === 200) {
                        showToast('Success', responseObject.responseMessage, 'success');
                        updatedRow = []; // Clear updatedRow after successful update
                    } else if (responseObject.statusCode === 500) {
                        showToast('Error', responseObject.responseMessage, 'error');
                    }
                },
                error: function (xhr, status, error) {
                    console.error("API error response:", xhr.responseText, status, error);
                    showToast('Error', 'An error occurred while updating data.', 'error');
                }
            });
        }

  
        
    });
});


// Function to manually add action buttons to the last column
function addActionButtons() {
    $('#spreadsheet tbody tr').each(function () {
        var $lastTd = $(this).find('td:last-child');
        if (!$lastTd.hasClass('action-buttons')) {
            $lastTd.addClass('action-buttons');
            $lastTd.html(`
                <button class="btn btn-sm btn-success add-row" title="Add Row">+</button>
                <button class="btn btn-sm btn-danger delete-row" title="Delete Row">-</button>
            `);
        }
    });
}


function retrivedObject(rowData) {
    return {
        id: rowData[0],
        monthId: rowData[1],
        journalMasterId: rowData[2],
        accountNumber: rowData[3],
        journalCode: rowData[4],
        details: rowData[5],
        debit: rowData[6],
        credit: rowData[7],
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
        url: '/api/Employees/GetEmployeeTypes',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            employeeTypes = responseObject.data;
            $('#EmployeeTypeId').empty();
            $.each(employeeTypes, function (key, item) {
                if (item.id == 1 || item.id == 2) {
                    $('#EmployeeTypeId').append(`<option value=${item.id}>${item.employeeTypeName}</option>`);
                }

            });
        },
        error: function (responseObject) {
        }
    });
}
