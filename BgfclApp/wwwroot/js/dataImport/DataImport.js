$(document).ready(function () {
    $('#importBtn').on('click', () => {
        console.log("Click");

        var file = $('#excelFileInput').prop('files')[0];

        if (!file) {
            alert('Please select an Excel file.');
            return;
        }

        var fileExtension = file.name.split('.').pop().toLowerCase();
        if (fileExtension !== 'xlsx' && fileExtension !== 'xls') {
            alert('Invalid file type. Please upload an Excel file.');
            return;
        }

        var selectedTable = $('#dataType').val();
        var apiEndpoint = getApiEndpoint(selectedTable);

        if (!apiEndpoint) {
            alert("Invalid selection. Please choose a valid table.");
            return;
        }

        let reader = new FileReader();
        reader.readAsArrayBuffer(file);
        reader.onload = function (e) {
            let data = new Uint8Array(e.target.result);
            let workbook = XLSX.read(data, { type: 'array' });
            let sheetName = workbook.SheetNames[0];
            let worksheet = workbook.Sheets[sheetName];

            let jsonData = XLSX.utils.sheet_to_json(worksheet, { header: 1 });
            let headers = jsonData[0];

            let dataList = [];
            for (var i = 1; i < jsonData.length; i++) {
                let row = jsonData[i];
                let rowData = {};

                headers.forEach((header, index) => {
                    let value = row[index];
                    if (header === "TaxStatus" || header === "ActiveStatus" || header === "IsActive" || header === "IsEmployee" || header === "IsAuthority" || header === "IsStaff") {
                        if (typeof value === "string") {
                            let lowerValue = value.trim().toLowerCase();
                            if (["true", "yes", "1"].includes(lowerValue)) {
                                value = true;
                            } else if (["false", "no", "0"].includes(lowerValue)) {
                                value = false;
                            } else {
                                value = null;
                            }
                        } else if (typeof value === "number") {
                            value = value === 1; // Convert 1 to true, 0 to false
                        } else {
                            value = null; // Ensure invalid types are set to null
                        }
                    }
                    if (header === "EmpSL" || header === "Nid" || header === "MultiDesignation") {
                        value = value !== undefined && value !== null ? String(value).trim() : "";
                    }
                    rowData[header] = value;
                });
                dataList.push(rowData);
            }

            console.log("Generated Data:", dataList);
            $.ajax({
                url: apiEndpoint,
                type: 'POST',
                async: true,
                contentType: 'application/json',
                data: JSON.stringify(dataList),
                success: function (responseObject) {
                    if (responseObject.statusCode === 200) {
                        showToast('Success', responseObject.responseMessage, 'success');
                    } else if (responseObject.statusCode === 500) {
                        showToast('Error', responseObject.responseMessage, 'error');
                    }
                },
                error: function (xhr, status, error) {
                    showToast('Error', 'An error occurred while processing the file.', 'error');
                    console.error("Error: " + error);
                }
            });

        };
    });

    function getApiEndpoint(tableType) {
        const apiMapping = {
            "1": "/api/DataImports/ImportEmployeeData",
            "2": "/api/DataImports/ImportDesignationData",
            "3": "/api/DataImports/ImportRoleData",
            "4": "/api/DataImports/ImportDepartmentData"
        };
        return apiMapping[tableType] || null;
    }
});
