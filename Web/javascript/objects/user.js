
//Password is being removed from UserEntity. 
function UserEntity(id,userName,firstName,middleName,lastName,title,isHomosapien, credentials)
{
	this.Id = id;
	this.UserName = userName;
	this.FirstName = firstName;
	this.MiddleName = middleName;
	this.LastName = lastName;
	this.Title = title;
	this.Credentials = credentials;
	this.IsSystemAccount = isHomosapien;
	UserEntity.prototype.toString = toString;
}

function toString() 
{
	return "Id:" + this.id + "\n" +
		   "UserName:" + this.userName + "\n" +
		   "FirstName:" + this.firstName + "\n" +
		   "MiddleName:" + this.middleName + "\n" +
		   "LastName:" + this.lastName + "\n" +
		   "Title:" + this.title + "\n" +
		   "IsSystemAccount:" + this.isHomosapien + 
		   "Credentials:" + this.credentials + "\n";
}