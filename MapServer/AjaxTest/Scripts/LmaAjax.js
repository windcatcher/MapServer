//发送ajax的XHR对象
function Ajax() {
    //XMLHTTP发送数据类型GET或POST
    this.method = "POST";
    //服务器URL
    this.URL = null;
    //指定同步或异步的读取方式,同步为true
    this.sync = false;
    //当method为POST时所要发送的数据
    this.PostData = null;
    //
    this.RetData = null;
    //
    this.OnResponse = null;
    //
    this.HttpObj = this.CreateXmlHttpRequest();
    if (this.HttpObj == null) {
        return;
    }
    
    var Obj = this;
    this.HttpObj.onreadystatechange = function () {
        Ajax.handleStateChange(Obj)        
    }}

    //产生XHR对象
    Ajax.prototype.CreateXmlHttpRequest = function () {
        if (window.XMLHttpRequest) {
            // code for IE7+, Firefox, Chrome, Opera, Safari
            return new XMLHttpRequest();
        } else if (window.ActiveXObject) {
            // code for IE6, IE5
            var msxmls = new Array('Msxml2.XMLHTTP.5.0', 'Msxml2.XMLHTTP.4.0'
            , 'Msxml2.XMLHTTP.3.0', 'Msxml2.XMLHTTP', 'Microsoft.XMLHTTP');
            for (var i = 0; i < msxmls.Length; i++) {
                try {
                    return new ActiveXObject(msxmls[i])
                } catch (e) {
                }
            }
        }
        return null;
    }

    //使用XHR发送HTTP请求
    Ajax.prototype.send = function () {
        if (this.HttpObj != null) {
            this.HttpObj.open(this.method, this.URL, this.sync); 
            if (this.method.toLocaleUpperCase() == "GET") {
                this.HttpObj.send(null);
            }
            else if (this.method.toLocaleUpperCase() == "POST") {
                this.HttpObj.setRequestHeader("Content-Type",
                "application/x-www-form-urlencoded");
                this.HttpObj.send(this.PostData);
            }
            else {
                return;
            }
        }
    }

    //事件检测
    Ajax.handleStateChange = function (Obj) {
        if (Obj.HttpObj.readyState == 4) {
            //判断对象状态
            if (Obj.HttpObj.status == 200) {
                Obj.RetData = Obj.HttpObj.responseText;
                if (Obj.OnResponse) {
                    Obj.OnResponse(Obj.RetData);
                }
                return;
            }
        }
    }


