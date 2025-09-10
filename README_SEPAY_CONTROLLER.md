# ??? SEPAY CONTROLLER - ADMIN MANAGEMENT ENDPOINTS

## ?? T?ng quan
SePayController riêng bi?t cho admin qu?n lý t?t c? các ch?c n?ng liên quan ??n SePay payment verification.

## ?? **Authentication Required:**
```
Authorization: Bearer <admin-token>
Role: Admin
```

## ?? **ENDPOINTS AVAILABLE:**

### 1. **Connection Testing**

#### Test SePay API Connection
```http
GET /api/SePay/test-connection
```
**Response:**
```json
{
    "success": true,
    "message": "SePay API is accessible",
    "data": "Connection successful",
    "statusCode": 200
}
```

### 2. **Transaction Management**

#### Get SePay Transactions
```http
GET /api/SePay/transactions
```
**Response:**
```json
{
    "success": true,
    "message": "Transaction list retrieved successfully",
    "data": {
        "status": 200,
        "transactions": [
            {
                "id": "22758505",
                "transaction_content": "Thanh Order number ORD00004",
                "amount_in": "30000.00",
                "transaction_date": "2025-09-09 13:23:00"
            }
        ]
    },
    "statusCode": 200
}
```

### 3. **Payment Verification Management**

#### Check Verification Status
```http
GET /api/SePay/verify-status/123
```
**Response:**
```json
{
    "success": true,
    "message": "Order is being verified",
    "data": {
        "orderId": 123,
        "isVerifying": true
    },
    "statusCode": 200
}
```

#### Stop Verification
```http
POST /api/SePay/stop-verification/123
```
**Response:**
```json
{
    "success": true,
    "message": "Payment verification stopped for order 123",
    "data": {
        "orderId": 123,
        "action": "Verification stopped"
    },
    "statusCode": 200
}
```

#### Manual Start Verification
```http
POST /api/SePay/start-verification/123?amount=30000&orderNumber=ORD00123
```
**Response:**
```json
{
    "success": true,
    "message": "Payment verification started for order 123",
    "data": {
        "orderId": 123,
        "amount": 30000,
        "orderNumber": "ORD00123",
        "action": "Verification started"
    },
    "statusCode": 200
}
```

### 4. **Debug Information**

#### Get Service Status
```http
GET /api/SePay/status
```
**Response:**
```json
{
    "success": true,
    "message": "SePay service status",
    "data": {
        "serviceName": "SePayService",
        "features": [
            "Transaction retrieval",
            "Payment verification",
            "Automatic verification",
            "Manual verification control"
        ],
        "endpoints": {
            "testConnection": "/api/SePay/test-connection",
            "getTransactions": "/api/SePay/transactions",
            "verifyStatus": "/api/SePay/verify-status/{orderId}",
            "stopVerification": "/api/SePay/stop-verification/{orderId}",
            "startVerification": "/api/SePay/start-verification/{orderId}?amount={amount}&orderNumber={orderNumber}"
        }
    },
    "statusCode": 200
}
```

## ??? **USE CASES:**

### 1. **Troubleshooting Payment Issues:**
```bash
# Check if SePay API is accessible
GET /api/SePay/test-connection

# Get recent transactions to see if payment came through
GET /api/SePay/transactions

# Check if order is being verified
GET /api/SePay/verify-status/123
```

### 2. **Manual Payment Recovery:**
```bash
# If automatic verification failed, start manual verification
POST /api/SePay/start-verification/123?amount=30000&orderNumber=ORD00123

# Monitor verification status
GET /api/SePay/verify-status/123

# Stop if needed
POST /api/SePay/stop-verification/123
```

### 3. **System Monitoring:**
```bash
# Check service health
GET /api/SePay/status

# Test connectivity
GET /api/SePay/test-connection

# View recent transactions
GET /api/SePay/transactions
```

## ?? **ADMIN WORKFLOW:**

### **Daily Operations:**
1. **Morning Check:** `GET /api/SePay/test-connection`
2. **Review Transactions:** `GET /api/SePay/transactions`
3. **Check Stuck Orders:** `GET /api/SePay/verify-status/{orderId}`

### **Issue Resolution:**
1. **Customer Reports Payment:** Check transactions
2. **Payment Not Detected:** Manual start verification
3. **System Issues:** Stop/restart verification

### **Debugging:**
1. **Test API connectivity**
2. **Check service status**
3. **Review transaction logs**
4. **Manual verification control**

## ?? **ERROR HANDLING:**

### **Common Responses:**
```json
// Cloudflare blocked
{
    "success": false,
    "message": "SePay API is protected by Cloudflare",
    "statusCode": 403
}

// Order already verifying
{
    "success": false,
    "message": "Order 123 is already being verified",
    "statusCode": 400
}

// Invalid parameters
{
    "success": false,
    "message": "Amount must be greater than 0",
    "statusCode": 400
}
```

## ?? **BENEFITS:**

### **For Admins:**
- ? **Complete control** over payment verification
- ? **Real-time monitoring** c?a verification status
- ? **Manual recovery** cho stuck payments
- ? **Debug tools** cho troubleshooting

### **For System:**
- ? **Separated concerns** - SePay logic riêng bi?t
- ? **Admin-only access** - secure management
- ? **Comprehensive endpoints** - ??y ?? ch?c n?ng
- ? **Easy integration** - RESTful API

**SePayController is now your one-stop shop for all SePay payment management! ???**