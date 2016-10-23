/*
 Author : Apurv Ghai
  Description: The program has been distributed *as is* to help the community members and do not certify to be used for Production Use

*/
function Test() {
    var clientUrl = Xrm.Page.context.getClientUrl();
    var req = new XMLHttpRequest()
    req.open("PATCH", encodeURI(clientUrl + "/api/data/v8.0/accounts(c9270348-57e6-e511-80cd-00155d4b0e09)"), true);
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.onreadystatechange = function () {
        if (this.readyState == 4 /* complete */) {
            req.onreadystatechange = null;
            if (this.status == 204) {

                //Confirmation Message
                Xrm.Utility.alertDialog("Done Deal")
            }
            else {
                debugger;
                var error = JSON.parse(this.response).error;
                Xrm.Utility.alertDialog("An Error Occurred");
            }
        }
    };
    //Updating New Value
    req.send(JSON.stringify({ name: "Sample account", telephone1: "011019" }));

}
