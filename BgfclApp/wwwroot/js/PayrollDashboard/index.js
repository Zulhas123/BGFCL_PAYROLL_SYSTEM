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
    getLastbonusMonth();
    getLastbonusAmounnt();
    getLastMonthAmentiesProcess();
    getLastMonthAmentiesAmount();
    getOfficerInfo();
    getJuniorStaffInfo();
    getPensionOFJSInfo();
    GetSalaryNetpayPermanent();
    GetSalaryNetpayContract();
    GetSalaryGrossPermanent();
    GetSalaryGrossContract();
    GetRevenueStampPermanent();
    GetRevenueStampContruct();
    GetPFPermanent();
    GetPFContruct();
    GetTotalDeductionPermanent();
    GetTotalDeductionContruct();
    getBranchInfo();
    getSchoolInfo();
    getRolelInfo();
    getLastPermanentEmployee();
    getLastContractEmployee();
    GetLastBonusAmount();
});

function getOfficerInfo() {
    $.ajax({
        url: '/api/Employees/GetAllOfficer',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            $('#officer_count').empty();
            $('#officer_count').text('Employee(P) : ' + responseObject.data.length);
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
            $('#staff_count').text('Employee(C) : ' + responseObject.data.length);
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
            $('#designation_count').text('Designation: ' + responseObject.data.length);
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
            $('#department_count').text('Department: ' + responseObject.data.length);
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
            $('#bank_count').text('Bank: ' + responseObject.data.length);
        },
        error: function (responseObject) {
        }
    });
}
function getBranchInfo() {
    $.ajax({
        url: '/api/Banks/GetBranches',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            $('#branch_count').empty();
            $('#branch_count').text(' Branch: ' + responseObject.data.length);
        },
        error: function (responseObject) {
        }
    });
}
function getSchoolInfo() {
    $.ajax({
        url: '/api/Schools/GetSchools',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            $('#school_count').empty();
            $('#school_count').text(' Schools: ' + responseObject.data.length);
        },
        error: function (responseObject) {
        }
    });
}
function getLastPermanentEmployee() {
    $.ajax({
        url: '/api/Employees/GetLastPermanetEmployee',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            $('#last_per_emp').empty();
            $('#last_per_emp').text(' New Permanent Employee: ' + responseObject.data);
        },
        error: function (responseObject) {
        }
    });
}
function GetLastBonusAmount() {
    $.ajax({
        url: '/api/Bonus/GetLatestBonusAmount',
        type: 'GET',
        dataType: 'json',
        success: function (responseObject) {
            console.log("bonus", responseObject)
            if (responseObject.data && responseObject.data.length > 0) {
                const bonus = responseObject.data[0];
                $('#bonusAmount').text('Total Bonus Issued:  ' + bonus.totalBonusAmount);
            } else {
                $('#bonusAmount').text('No bonus data found.');
            }
        },
        error: function () {
            $('#bonusAmount').text('Error fetching bonus data.');
        }
    });
}

function getLastContractEmployee() {
    $.ajax({
        url: '/api/Employees/GetLastContractEmployee',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            $('#last_cont_emp').empty();
            $('#last_cont_emp').text(' New Contract Employee: ' + responseObject.data);
        },
        error: function (responseObject) {
        }
    });
}
function getRolelInfo() {
    $.ajax({
        url: '/api/Roles/GetRole',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            $('#role_count').empty();
            $('#role_count').text(' Roles: ' + responseObject.data.length);
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
            $('#grade_count').text('Grade: ' + responseObject.data.length);
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
function GetSalaryNetpayPermanent() {
    $.ajax({
        url: '/api/SalarySettings/GetSalaryNetpayPermanent',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            if (responseObject.statusCode === 200 && responseObject.data) {
                const NetAmount = responseObject.data;
                $('#netAmountPer').text(NetAmount);
            } else {
                console.error('Failed to fetch data or no data returned.');
            }
        },
        error: function (responseObject) {
            console.error('Error in API call:', responseObject);
        }
    });
}
function GetSalaryGrossPermanent() {
    $.ajax({
        url: '/api/SalarySettings/GetSalaryGrossPermanent',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            if (responseObject.statusCode === 200 && responseObject.data) {
                const grossAmount = responseObject.data;
                $('#grossAmountPer').text(grossAmount);
            } else {
                console.error('Failed to fetch data or no data returned.');
            }
        },
        error: function (responseObject) {
            console.error('Error in API call:', responseObject);
        }
    });
}
function GetSalaryGrossContract() {
    $.ajax({
        url: '/api/SalarySettings/GetSalaryGrossContract',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            if (responseObject.statusCode === 200 && responseObject.data) {
                const grossAmount = responseObject.data;
                $('#grossAmountCont').text(grossAmount);
            } else {
                console.error('Failed to fetch data or no data returned.');
            }
        },
        error: function (responseObject) {
            console.error('Error in API call:', responseObject);
        }
    });
}
function GetSalaryNetpayContract() {
    $.ajax({
        url: '/api/SalarySettings/GetSalaryNetpayContract',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            if (responseObject.statusCode === 200 && responseObject.data) {
                const NetAmount = responseObject.data;
                $('#netAmountCont').text(NetAmount);
            } else {
                console.error('Failed to fetch data or no data returned.');
            }
        },
        error: function (responseObject) {
            console.error('Error in API call:', responseObject);
        }
    });
}
function GetRevenueStampPermanent() {
    $.ajax({
        url: '/api/SalarySettings/GetRevenueStampPermanent',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            if (responseObject.statusCode === 200 && responseObject.data) {
                const revenueStamp = responseObject.data;
                $('#revenueStampPer').text(revenueStamp);
            } else {
                console.error('Failed to fetch data or no data returned.');
            }
        },
        error: function (responseObject) {
            console.error('Error in API call:', responseObject);
        }
    });
}
function GetRevenueStampContruct() {
    $.ajax({
        url: '/api/SalarySettings/GetRevenueStampContruct',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            if (responseObject.statusCode === 200 && responseObject.data) {
                const revenueStamp = responseObject.data;
                $('#revenueStampCont').text(revenueStamp);
            } else {
                console.error('Failed to fetch data or no data returned.');
            }
        },
        error: function (responseObject) {
            console.error('Error in API call:', responseObject);
        }
    });
}
function GetPFPermanent() {
    $.ajax({
        url: '/api/SalarySettings/GetPFPermanent',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            if (responseObject.statusCode === 200 && responseObject.data) {
                const pfP = responseObject.data;
                $('#pfP').text(pfP);
            } else {
                console.error('Failed to fetch data or no data returned.');
            }
        },
        error: function (responseObject) {
            console.error('Error in API call:', responseObject);
        }
    });
}
function GetPFContruct() {
    $.ajax({
        url: '/api/SalarySettings/GetPFContruct',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            if (responseObject.statusCode === 200 && responseObject.data) {
                const pfC = responseObject.data;
                $('#pfC').text(pfC);
            } else {
                console.error('Failed to fetch data or no data returned.');
            }
        },
        error: function (responseObject) {
            console.error('Error in API call:', responseObject);
        }
    });
}
function GetTotalDeductionPermanent() {
    $.ajax({
        url: '/api/SalarySettings/GetTotalDeductionPermanent',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            if (responseObject.statusCode === 200 && responseObject.data) {
                const deductionp = responseObject.data;
                $('#deductionp').text(deductionp);
            } else {
                console.error('Failed to fetch data or no data returned.');
            }
        },
        error: function (responseObject) {
            console.error('Error in API call:', responseObject);
        }
    });
}
function GetTotalDeductionContruct() {
    $.ajax({
        url: '/api/SalarySettings/GetTotalDeductionContruct',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            if (responseObject.statusCode === 200 && responseObject.data) {
                const deductionc = responseObject.data;
                $('#deductionc').text(deductionc);
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

function getLastbonusMonth() {
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
function getLastbonusName() {
    $.ajax({
        url: '/api/Bonus/GetLastBonusName',
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            if (responseObject.statusCode === 200 && responseObject.data) {
                const bonusname = responseObject.data.bonusTitle;
                console.log("bonusname", bonusname)
                $('#latestbonusName').text(bonusname);
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

