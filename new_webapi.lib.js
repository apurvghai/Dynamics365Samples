/*
 Author : Apurv Ghai
  Description: The program has been distributed *as is* to help the community members and do not certify to be used for Production Use.

*/

src = "ClientGlobalContext.js.aspx";

function _getContext() {
    var errorMessage = "Context is not available.";
    if (typeof GetGlobalContext != "undefined")
    { return GetGlobalContext(); }
    else
    {
        if (typeof Xrm != "undefined") {
            return Xrm.Page.context;
        }
        else { throw new Error(errorMessage); }
    }
}

function UpdateAccount() {
    var clientUrl = Xrm.Page.context.getClientUrl();
    var req = new XMLHttpRequest()
    req.open("PATCH", encodeURI(clientUrl + "/api/data/v8.2/accounts(c9270348-57e6-e511-80cd-00155d4b0e09)"), true);
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.onreadystatechange = function () {
        if (this.readyState == 4 /* complete */) {
            req.onreadystatechange = null;
            if (this.status == 204) {

                //Confirmation Message
                Xrm.Utility.alertDialog("Account has been updated.")
            }
            else {
                //debugger;
                var error = JSON.parse(this.response).error;
                Xrm.Utility.alertDialog("An Error Occurred");
            }
        }
    };
    //Updating New Value
    req.send(JSON.stringify({ name: "Sample account", telephone1: "011019" }));

}

function CreateAccount(strAccountName) {
    var clientUrl = Xrm.Page.context.getClientUrl();
    var req = new XMLHttpRequest()
    req.open("POST", encodeURI(clientUrl + "/api/data/v8.2/accounts"), true);
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.onreadystatechange = function () {
        if (this.readyState == 4 /* complete */) {
            req.onreadystatechange = null;
            if (this.status == 204) {

                //Confirmation Message
                Xrm.Utility.alertDialog("Account has been created.")
            }
            else {
                debugger;
                var error = JSON.parse(this.response).error;
                Xrm.Utility.alertDialog("An Error Occurred creating account");
            }
        }
    };
    //Updating New Value
    req.send(JSON.stringify({ name: strAccountName, telephone1: "000-000-0000" }));
}


//This function is used to search strings. It reads the textbox value using DOM. 
//The script below is used for Demo in HTML WebResource
function SearchAccounts() {

    var strSearch = document.getElementById("txtAccountName").value;
    var clientUrl = Xrm.Page.context.getClientUrl();
    var req = new XMLHttpRequest();

    req.open("GET", encodeURI(clientUrl + "/api/data/v8.2/accounts?$select=name&$filter=contains(name,'" + strSearch + "')"));
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.onreadystatechange = function () {
        if (this.readyState == 4) {
            req.onreadystatechange = null;
            if (this.status == 200) {
                var data = JSON.parse(this.response);
                var intDataCount = data.value.length;
                alert(intDataCount + " account(s) found with the string " + strSearch);
                if (intDataCount > 0) {


                    if (data && data.value) {
                        for (var accountCount = 0; accountCount < data.value.length; accountCount++) {
                            var accountName = data.value[accountCount].name;
                            var eTag = data.value[accountCount]['@odata.etag'];
                        }
                    }

                    else {
                        var error = JSON.parse(this.response).error;
                        Xrm.Utility.alertDialog("An Error Occurred");
                    }
                } else {
                    //if the account does not exist
                    CreateIfNotAvailable(strSearch);
                }
            }
        }
    };
    req.send(null);
}

function CreateIfNotAvailable(strSearch) {
    var doCreateConfirm = confirm("Account with name" + strSearch + " does not exist. Would you like to create it?");
    if (doCreateConfirm) {
        CreateAccount(strSearch);
    }
    else {
        alert("Alright, no problem!");
    }

}

