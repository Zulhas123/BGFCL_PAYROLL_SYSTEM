$(document).ready(function () {    
    getDesignationInfo();
    getDepartmentInfo();
    getBankTagInfo();
    getBankInfo();
    getLocationInfo();    
    getGradeInfo();
    getSalaryInfo();
    getSalaryNetpayOf();
    getSalaryNetpayJS();
    getLastbonusName();
    getLastbonusAmounnt();
    getLastMonthAmentiesProcess();
    getLastMonthAmentiesAmount();
    getOfficerInfo();
    getJuniorStaffInfo();
    getPensionOFJSInfo();
});

function getOfficerInfo() {
    $.ajax({
        url: '/api/Employees/GetAllOfficer',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            $('#officer_count').empty();
            $('#officer_count').text('Total Officer: ' + responseObject.data.length);
        },
        error: function (responseObject) {
        }
    });
}
function getJuniorStaffInfo() {
    $.ajax({
        url: '/api/Employees/GetAllJuniorStaff',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            $('#staff_count').empty();
            $('#staff_count').text('Total Junior Staff: ' + responseObject.data.length);
        },
        error: function (responseObject) {
        }
    });
}
function getPensionOFJSInfo() {
    $.ajax({
        url: '/api/Employees/GetAllPensionOFJS',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            $('#pension_count').empty();
            $('#pension_count').text('Pension OF & JS: ' + responseObject.data.length);
        },
        error: function (responseObject) {
        }
    });
}
function getDesignationInfo() {
    $.ajax({
        url: '/api/Designations/GetDesignations',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            $('#designation_count').empty();
            $('#designation_count').text('Total Designation: ' + responseObject.data.length);
        },
        error: function (responseObject) {
        }
    });
}

function getDepartmentInfo() {
    $.ajax({
        url: '/api/Departments/GetDepartments',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            $('#department_count').empty();
            $('#department_count').text('Total Department: ' + responseObject.data.length);
        },
        error: function (responseObject) {
        }
    });
}

function getBankTagInfo() {
    $.ajax({
        url: '/api/Banks/GetBankTags',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            $('#bank_tag_count').empty();
            $('#bank_tag_count').text('Total Bank Tags: ' + responseObject.data.length);
        },
        error: function (responseObject) {
        }
    });
}

function getBankInfo() {
    $.ajax({
        url: '/api/Banks/GetBanks',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            $('#bank_count').empty();
            $('#bank_count').text('Total Bank: ' + responseObject.data.length);
        },
        error: function (responseObject) {
        }
    });
}

function getLocationInfo() {
    $.ajax({
        url: '/api/Locations/GetLocations',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            $('#location_count').empty();
            $('#location_count').text('Total Location: ' + responseObject.data.length);
        },
        error: function (responseObject) {
        }
    });
}

function getGradeInfo() {
    console.log('test');
    $.ajax({
        url: '/api/Grades/GetGrades',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            $('#grade_count').empty();
            $('#grade_count').text('Total Grade: ' + responseObject.data.length);
            console.log('last call');
        },
        error: function (responseObject) {
        }
    });
}

function getSalaryInfo() {
    console.log('call end');
    $.ajax({
        url: '/api/SalarySettings/LatestSalaryProcess',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            debugger
            console.log('data1: ', responseObject);
            if (responseObject.statusCode === 200 && responseObject.data) {
                console.log('data: ', responseObject.data);
                const latestMonth = responseObject.data;
                $('#latestMonthValue').text(latestMonth);
            } else {
                console.error('Failed to fetch data or no data returned.');
            }
        },
        error: function (responseObject) {
            console.error('Error in API call:', responseObject);
        }
    });
}

function getSalaryNetpayOf() {
    $.ajax({
        url: '/api/SalarySettings/GetNetpayOf',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            console.log('data1: ', responseObject);
            if (responseObject.statusCode === 200 && responseObject.data) {
                const NetAmount = responseObject.data;
                $('#totalAmountOF').text(NetAmount);
            } else {
                console.error('Failed to fetch data or no data returned.');
            }
        },
        error: function (responseObject) {
            console.error('Error in API call:', responseObject);
        }
    });
}

function getSalaryNetpayJS() {
    $.ajax({
        url: '/api/SalarySettings/GetNetpayJs',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            console.log('data1: ', responseObject);
            if (responseObject.statusCode === 200 && responseObject.data) {
                const NetAmount = responseObject.data;
                $('#totalAmountJS').text(NetAmount);
            } else {
                console.error('Failed to fetch data or no data returned.');
            }
        },
        error: function (responseObject) {
            console.error('Error in API call:', responseObject);
        }
    });
}

function getLastbonusName() {
    $.ajax({
        url: '/api/Bonus/GetLastBonus',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            if (responseObject.statusCode === 200 && responseObject.data) {
                const bonusmonth = responseObject.data;
                $('#latestbonusMonth').text(bonusmonth);
            } else {
                console.error('Failed to fetch data or no data returned.');
            }
        },
        error: function (responseObject) {
            console.error('Error in API call:', responseObject);
        }
    });
}
function getLastbonusAmounnt() {
    $.ajax({
        url: '/api/Bonus/GetLastBonusAmount',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            if (responseObject.statusCode === 200 && responseObject.data) {
                const bonusAmount = responseObject.data;
                $('#totalBonus').text(bonusAmount);
            } else {
                console.error('Failed to fetch data or no data returned.');
            }
        },
        error: function (responseObject) {
            console.error('Error in API call:', responseObject);
        }
    });
}

function getLastMonthAmentiesProcess() {
    $.ajax({
        url: '/api/Amenities/GetAmenitiesLastMonthProcess',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            if (responseObject.statusCode === 200 && responseObject.data) {
                const amemonth = responseObject.data;
                $('#latestAmeMonth').text(amemonth);
            } else {
                console.error('Failed to fetch data or no data returned.');
            }
        },
        error: function (responseObject) {
            console.error('Error in API call:', responseObject);
        }
    });
}


function getLastMonthAmentiesAmount() {
    $.ajax({
        url: '/api/Amenities/GetAmenitiesLastMonthAmount',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            if (responseObject.statusCode === 200 && responseObject.data) {
                
                const ameamount = responseObject.data;
                $('#totalAme').text(ameamount);
            } else {
                console.error('Failed to fetch data or no data returned.');
            }
        },
        error: function (responseObject) {
            console.error('Error in API call:', responseObject);
        }
    });
}

