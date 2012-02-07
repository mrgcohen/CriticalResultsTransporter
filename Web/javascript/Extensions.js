var Extensions =
{
	extensions: {}

};
Extensions.register = function(name, object)
{
	if (typeof (extensions[name]) == "undefined")
	{
		this.extensions[name] = [object];
	} else
	{
		this.extensions.push(object);
	}
};

