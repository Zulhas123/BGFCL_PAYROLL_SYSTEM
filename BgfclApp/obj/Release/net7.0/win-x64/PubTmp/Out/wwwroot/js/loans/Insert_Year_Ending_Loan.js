$(document).ready(function () {

    $('#process_button').on('click', () => {
        console.log("Click")
        var file = $('#excelFileInput').prop('files')[0];

        // Check if a file is selected
        if (!file) {
            alert('Please select an Excel file.');
            return;
        }

        // Check for valid Excel file type
        var fileExtension = file.name.split('.').pop().toLowerCase();
        if (fileExtension !== 'xlsx' && fileExtension !== 'xls') {
            alert('Invalid file type. Please upload an Excel file.');
            return;
        }

        let dataObj = {};

        let reader = new FileReader();
        reader.readAsArrayBuffer(file);
        reader.onload = function (e) {
            let data = new Uint8Array(e.target.result);
            let workbook = XLSX.read(data, { type: 'array' });
            let sheetName = workbook.SheetNames[0]; 
            let worksheet = workbook.Sheets[sheetName];
            let jsonData = XLSX.utils.sheet_to_json(worksheet, { header: 1 });

            let dataList = [];
            for (var i = 1; i < jsonData.length; i++) {
                let jobCode = jsonData[i][0];
                let amount = jsonData[i][1];
                let interest = jsonData[i][2];
                let type = jsonData[i][3];
                if (!jobCode || isNaN(amount)) {
                    continue;
                }

                dataList.push({
                    jobCode: jobCode,
                    amount: amount,
                    type: type,
                    interest:interest
                });
            }

            dataObj.loanData = dataList;
            console.log("Object data", dataObj)

            $.ajax({
                url: '/api/Loans/UpdateYearEndingData',
                type: 'POST',
                async: true, 
                contentType: 'application/json',
                data: JSON.stringify(dataObj.loanData), 
                success: function (responseObject) {
                    if (responseObject.statusCode === 201) {
                        showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    } else if (responseObject.statusCode === 500) {
                        showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                    }
                },
                error: function (xhr, status, error) {
                    showToast(title = 'Error', message = 'An error occurred while processing the file.', toastrType = 'error');
                    console.error("Error: " + error);
                }
            });
        };
    });
});
