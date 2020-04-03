class User {
	public Id: number;
	public Email: string;
	public FirstName: string;
	public LastName: string;
	public Username: string;
	public HashedPassword: string;
	public ValidatedEmail: boolean;
	public Address: string;
	public Lat: number;
	public Lng: number;
	public Description: string;
}

export default User;