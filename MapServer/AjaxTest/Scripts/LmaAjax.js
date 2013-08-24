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
    ths.HttpObj = this.CreateXmlHttpRequest();
    if (this.HttpObj == null) {
        return;
    }
    var Obj = this;
    this.HttpObj.onreadystatechange = function () {
        Ajax.handleStateChanget(Obj)
    }

    //产生XHR对象
    Ajax.prototype.CreateXmlHttpRequest = function () {
        if (window.XMLHttpRequest) {
            return new XMLHttpRequest();
        } else if (window.ActiveXObject) {
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
            this.HttpObj.Open(this.method, this.URL, this.sync); ;
            if (this.method.toLocaleUpperCase()=="GET") {
            }
        }

    }
    //事件检测
}