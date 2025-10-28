# Profile Management API - User Guide

## 🎯 Overview

The AmbulanceRider API now includes comprehensive profile management endpoints that allow authenticated users to:
- ✅ Update their profile information (name, phone)
- ✅ Change their password
- ✅ Upload/update profile image
- ✅ Remove profile image

All endpoints require JWT authentication and operate on the currently logged-in user.

---

## 🔐 Authentication Required

All profile management endpoints require a valid JWT Bearer token. 

**How to authenticate:**
1. Login via `/api/auth/login`
2. Copy the `accessToken` from the response
3. Include in all requests: `Authorization: Bearer {your-token}`

---

## 📋 Available Endpoints

### 1. Update Profile Information
**Endpoint:** `PUT /api/auth/profile`  
**Authentication:** Required  
**Content-Type:** `application/json`

Updates the current user's profile information. Only provided fields will be updated.

#### Request Body
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+254712345678"
}
```

#### Response (200 OK)
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "name": "John Doe",
  "firstName": "John",
  "lastName": "Doe",
  "email": "user@example.com",
  "phoneNumber": "+254712345678",
  "imagePath": null,
  "imageUrl": null,
  "roles": ["User"],
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-10-28T16:25:00Z"
}
```

#### Notes
- All fields are optional
- Empty or null fields will be ignored
- Email cannot be changed through this endpoint
- Roles cannot be changed through this endpoint

---

### 2. Change Password
**Endpoint:** `POST /api/auth/change-password`  
**Authentication:** Required  
**Content-Type:** `application/json`

Changes the current user's password. Requires current password for verification.

#### Request Body
```json
{
  "currentPassword": "OldPassword@123",
  "newPassword": "NewSecurePass@456",
  "confirmPassword": "NewSecurePass@456"
}
```

#### Response (200 OK)
```json
{
  "message": "Password changed successfully"
}
```

#### Password Requirements
- ✅ Minimum 8 characters
- ✅ At least one uppercase letter
- ✅ At least one lowercase letter
- ✅ At least one digit
- ✅ At least one special character

#### Error Response (400 Bad Request)
```json
{
  "message": "Failed to change password",
  "errors": [
    {
      "code": "PasswordMismatch",
      "description": "Current password is incorrect"
    }
  ]
}
```

---

### 3. Upload Profile Image
**Endpoint:** `POST /api/auth/profile/image`  
**Authentication:** Required  
**Content-Type:** `multipart/form-data`

Uploads a new profile image for the current user.

#### Request (Form Data)
```
image: [File]
```

#### Image Requirements
- **Allowed formats:** JPG, JPEG, PNG, GIF
- **Maximum size:** 5MB
- **Previous image:** Automatically deleted

#### Response (200 OK)
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "name": "John Doe",
  "firstName": "John",
  "lastName": "Doe",
  "email": "user@example.com",
  "phoneNumber": "+254712345678",
  "imagePath": "/path/to/image.jpg",
  "imageUrl": "/uploads/profiles/123e4567_abc123.jpg",
  "roles": ["User"],
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-10-28T16:25:00Z"
}
```

#### Error Responses

**Invalid Format (400 Bad Request)**
```json
{
  "message": "Invalid image format. Allowed formats: jpg, jpeg, png, gif"
}
```

**File Too Large (400 Bad Request)**
```json
{
  "message": "Image size must be less than 5MB"
}
```

**No File Provided (400 Bad Request)**
```json
{
  "message": "No image file provided"
}
```

---

### 4. Remove Profile Image
**Endpoint:** `DELETE /api/auth/profile/image`  
**Authentication:** Required

Removes the current user's profile image. The image file is deleted from the server.

#### Response (200 OK)
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "name": "John Doe",
  "firstName": "John",
  "lastName": "Doe",
  "email": "user@example.com",
  "phoneNumber": "+254712345678",
  "imagePath": null,
  "imageUrl": null,
  "roles": ["User"],
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-10-28T16:25:00Z"
}
```

---

## 💻 Code Examples

### JavaScript/TypeScript (Axios)

#### Update Profile
```typescript
const updateProfile = async (profileData) => {
  const response = await axios.put(
    'http://localhost:5000/api/auth/profile',
    profileData,
    {
      headers: {
        'Authorization': `Bearer ${accessToken}`,
        'Content-Type': 'application/json'
      }
    }
  );
  return response.data;
};

// Usage
const updatedUser = await updateProfile({
  firstName: 'John',
  lastName: 'Doe',
  phoneNumber: '+254712345678'
});
```

#### Change Password
```typescript
const changePassword = async (passwords) => {
  const response = await axios.post(
    'http://localhost:5000/api/auth/change-password',
    passwords,
    {
      headers: {
        'Authorization': `Bearer ${accessToken}`,
        'Content-Type': 'application/json'
      }
    }
  );
  return response.data;
};

// Usage
await changePassword({
  currentPassword: 'OldPassword@123',
  newPassword: 'NewSecurePass@456',
  confirmPassword: 'NewSecurePass@456'
});
```

#### Upload Profile Image
```typescript
const uploadProfileImage = async (file) => {
  const formData = new FormData();
  formData.append('image', file);

  const response = await axios.post(
    'http://localhost:5000/api/auth/profile/image',
    formData,
    {
      headers: {
        'Authorization': `Bearer ${accessToken}`,
        'Content-Type': 'multipart/form-data'
      }
    }
  );
  return response.data;
};

// Usage with file input
const fileInput = document.getElementById('profileImage');
const file = fileInput.files[0];
const updatedUser = await uploadProfileImage(file);
```

#### Remove Profile Image
```typescript
const removeProfileImage = async () => {
  const response = await axios.delete(
    'http://localhost:5000/api/auth/profile/image',
    {
      headers: {
        'Authorization': `Bearer ${accessToken}`
      }
    }
  );
  return response.data;
};

// Usage
const updatedUser = await removeProfileImage();
```

---

### C# (.NET)

#### Update Profile
```csharp
public async Task<UserDto> UpdateProfileAsync(UpdateProfileDto profileDto)
{
    var client = new HttpClient();
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", accessToken);

    var json = JsonSerializer.Serialize(profileDto);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var response = await client.PutAsync(
        "http://localhost:5000/api/auth/profile", 
        content);
    
    response.EnsureSuccessStatusCode();
    
    var result = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<UserDto>(result);
}
```

#### Change Password
```csharp
public async Task ChangePasswordAsync(ChangePasswordDto passwordDto)
{
    var client = new HttpClient();
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", accessToken);

    var json = JsonSerializer.Serialize(passwordDto);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var response = await client.PostAsync(
        "http://localhost:5000/api/auth/change-password", 
        content);
    
    response.EnsureSuccessStatusCode();
}
```

#### Upload Profile Image
```csharp
public async Task<UserDto> UploadProfileImageAsync(Stream imageStream, string fileName)
{
    var client = new HttpClient();
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", accessToken);

    using var content = new MultipartFormDataContent();
    var fileContent = new StreamContent(imageStream);
    fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
    content.Add(fileContent, "image", fileName);

    var response = await client.PostAsync(
        "http://localhost:5000/api/auth/profile/image", 
        content);
    
    response.EnsureSuccessStatusCode();
    
    var result = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<UserDto>(result);
}
```

---

### cURL Examples

#### Update Profile
```bash
curl -X PUT http://localhost:5000/api/auth/profile \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "phoneNumber": "+254712345678"
  }'
```

#### Change Password
```bash
curl -X POST http://localhost:5000/api/auth/change-password \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "currentPassword": "OldPassword@123",
    "newPassword": "NewSecurePass@456",
    "confirmPassword": "NewSecurePass@456"
  }'
```

#### Upload Profile Image
```bash
curl -X POST http://localhost:5000/api/auth/profile/image \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -F "image=@/path/to/image.jpg"
```

#### Remove Profile Image
```bash
curl -X DELETE http://localhost:5000/api/auth/profile/image \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

---

## 🧪 Testing with Swagger UI

### 1. Authenticate
1. Navigate to `http://localhost:5000`
2. Expand **Authentication** section
3. Use `POST /api/auth/login` with test credentials
4. Copy the `accessToken`
5. Click **Authorize** button (top right)
6. Enter: `Bearer {your-token}`
7. Click **Authorize** then **Close**

### 2. Update Profile
1. Expand `PUT /api/auth/profile`
2. Click **Try it out**
3. Modify the JSON:
   ```json
   {
     "firstName": "Updated",
     "lastName": "Name",
     "phoneNumber": "+254700000000"
   }
   ```
4. Click **Execute**
5. View updated profile in response

### 3. Change Password
1. Expand `POST /api/auth/change-password`
2. Click **Try it out**
3. Enter passwords:
   ```json
   {
     "currentPassword": "Admin@123",
     "newPassword": "NewPassword@456",
     "confirmPassword": "NewPassword@456"
   }
   ```
4. Click **Execute**
5. Check for success message

### 4. Upload Profile Image
1. Expand `POST /api/auth/profile/image`
2. Click **Try it out**
3. Click **Choose File** and select an image
4. Click **Execute**
5. View `imageUrl` in response

### 5. Remove Profile Image
1. Expand `DELETE /api/auth/profile/image`
2. Click **Try it out**
3. Click **Execute**
4. Verify `imageUrl` is null in response

---

## 🔒 Security Considerations

### Authentication
- ✅ All endpoints require valid JWT token
- ✅ Users can only modify their own profile
- ✅ Token must be included in Authorization header

### Password Changes
- ✅ Current password required for verification
- ✅ Password complexity enforced
- ✅ Confirmation password must match
- ✅ Failed attempts logged

### Image Uploads
- ✅ File type validation (jpg, jpeg, png, gif only)
- ✅ File size limit (5MB maximum)
- ✅ Unique file names to prevent conflicts
- ✅ Old images automatically deleted
- ✅ Stored in secure uploads directory

### Data Validation
- ✅ Input sanitization
- ✅ Required field validation
- ✅ Format validation (phone numbers, etc.)
- ✅ Error messages don't expose sensitive info

---

## ❌ Common Errors

### 401 Unauthorized
**Cause:** Missing or invalid authentication token

**Solution:**
- Ensure you're logged in
- Check token hasn't expired (24 hours)
- Verify token format: `Bearer {token}`
- Re-login if token expired

### 400 Bad Request - Password Change
**Cause:** Current password incorrect or new password doesn't meet requirements

**Solution:**
- Verify current password is correct
- Ensure new password meets all requirements
- Check that new password and confirm password match

### 400 Bad Request - Image Upload
**Cause:** Invalid image format or size

**Solution:**
- Use only JPG, JPEG, PNG, or GIF formats
- Ensure image is less than 5MB
- Check file is not corrupted

### 400 Bad Request - Profile Update
**Cause:** Invalid data format

**Solution:**
- Check JSON is properly formatted
- Verify phone number format
- Ensure required fields are not empty strings

---

## 📊 Response Status Codes

| Code | Meaning | Description |
|------|---------|-------------|
| 200 | OK | Request successful |
| 400 | Bad Request | Invalid input data |
| 401 | Unauthorized | Not authenticated or invalid token |
| 404 | Not Found | User not found |
| 500 | Server Error | Internal server error |

---

## 🎯 Best Practices

### Profile Updates
1. **Partial Updates**: Only send fields you want to update
2. **Validation**: Validate data on client before sending
3. **Error Handling**: Always handle potential errors
4. **User Feedback**: Show success/error messages to users

### Password Changes
1. **Confirmation**: Always require password confirmation
2. **Strength Indicator**: Show password strength to users
3. **Success Action**: Log user out or require re-authentication
4. **Security**: Never log or store passwords in plain text

### Image Uploads
1. **Preview**: Show image preview before upload
2. **Compression**: Consider compressing images client-side
3. **Progress**: Show upload progress for large files
4. **Validation**: Validate file type and size client-side first
5. **Cleanup**: Remove old images when uploading new ones

---

## 🔄 Workflow Examples

### Complete Profile Setup
```typescript
// 1. Login
const authResponse = await login(email, password);
const token = authResponse.accessToken;

// 2. Update profile information
await updateProfile({
  firstName: 'John',
  lastName: 'Doe',
  phoneNumber: '+254712345678'
});

// 3. Upload profile image
const file = document.getElementById('imageInput').files[0];
await uploadProfileImage(file);

// 4. Verify profile
const currentUser = await getCurrentUser();
console.log('Profile complete:', currentUser);
```

### Password Change Flow
```typescript
// 1. Prompt user for passwords
const passwords = {
  currentPassword: prompt('Enter current password'),
  newPassword: prompt('Enter new password'),
  confirmPassword: prompt('Confirm new password')
};

// 2. Validate passwords match
if (passwords.newPassword !== passwords.confirmPassword) {
  alert('Passwords do not match');
  return;
}

// 3. Change password
try {
  await changePassword(passwords);
  alert('Password changed successfully');
  
  // 4. Optionally log user out
  logout();
} catch (error) {
  alert('Failed to change password: ' + error.message);
}
```

---

## 📝 Notes

- **UpdatedAt Field**: Automatically updated on all profile changes
- **Image Storage**: Images stored in `wwwroot/uploads/profiles/`
- **Image URLs**: Accessible via `/uploads/profiles/{filename}`
- **Concurrent Updates**: Last write wins (no conflict resolution)
- **Audit Trail**: Consider logging profile changes for security

---

## 🆘 Support

For issues or questions:
- Check the [SWAGGER_DOCUMENTATION.md](./SWAGGER_DOCUMENTATION.md) for API details
- Review [API_DOCUMENTATION.md](./API_DOCUMENTATION.md) for all endpoints
- Test endpoints in Swagger UI at `http://localhost:5000`
- Contact: support@ambulancerider.com

---

**Last Updated:** 2025-10-28  
**API Version:** 1.0.0  
**Status:** ✅ Production Ready
