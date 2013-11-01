function Student(name, age) {
    var _name = null;
    var _age = null;
    //私有的构造函数
    var initial = function ()
    {
        _name = name;
        _age = age;
    }

    //对象的属性this.get_name是公有的
    this.get_name = function () {
        return _name;
    }
    this.set_name = function (value) {
        _name = value;
    }

    this.get_age = function () {
        return _age;
    }
    this.set_age = function (value) {
        _age = value;
    }

    this.Sex = null;
   
    this.toString = function () {
        return this.get_name() + ",age is " + this.get_age() + ",sex is " + this.Sex;
    }
    //在类的最后调用init方法
    initial();
}