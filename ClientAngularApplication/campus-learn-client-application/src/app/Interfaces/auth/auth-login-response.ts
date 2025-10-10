export interface AuthLoginResponse
{
    token: string,
    user: {
        userProfileID: number,
        name: string,
        surname: string,
        email: string,
        userRole: number,
        qualification: number
    }
}