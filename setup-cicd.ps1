# Setup CICD - OIDC Configuration for GitHub Actions

# Get AWS Account ID
$ACCOUNT_ID = aws sts get-caller-identity --query Account --output text
Write-Host "AWS Account ID: $ACCOUNT_ID" -ForegroundColor Green

# Step 1: Create OIDC Provider
Write-Host "`nStep 1: Creating OIDC Provider..." -ForegroundColor Yellow
$OIDC_RESULT = aws iam create-open-id-connect-provider `
  --url https://token.actions.githubusercontent.com `
  --client-id-list sts.amazonaws.com `
  --thumbprint-list 1b511abead59c6ce207077c0bf4989fd6af1728d

$OIDC_ARN = $OIDC_RESULT | ConvertFrom-Json | Select-Object -ExpandProperty OpenIDConnectProviderArn
Write-Host "OIDC Provider created: $OIDC_ARN" -ForegroundColor Green

# Step 2: Create Trust Policy
Write-Host "`nStep 2: Creating Trust Policy..." -ForegroundColor Yellow
$TRUST_POLICY = @"
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": {
        "Federated": "arn:aws:iam::$($ACCOUNT_ID):oidc-provider/token.actions.githubusercontent.com"
      },
      "Action": "sts:AssumeRoleWithWebIdentity",
      "Condition": {
        "StringEquals": {
          "token.actions.githubusercontent.com:aud": "sts.amazonaws.com"
        },
        "StringLike": {
          "token.actions.githubusercontent.com:sub": "repo:hectorlaris/anti-corruption-layer-pattern:*"
        }
      }
    }
  ]
}
"@

$TRUST_POLICY | Out-File github-policy.json -Encoding UTF8
Write-Host "Trust policy created" -ForegroundColor Green

# Step 3: Create Role
Write-Host "`nStep 3: Creating IAM Role..." -ForegroundColor Yellow
try {
    aws iam create-role `
      --role-name GitHubActionsACLRole `
      --assume-role-policy-document file://github-policy.json | Out-Null
    Write-Host "Role created: GitHubActionsACLRole" -ForegroundColor Green
} catch {
    Write-Host "Role might already exist, continuing..." -ForegroundColor Yellow
}

# Step 4: Attach Policy
Write-Host "`nStep 4: Attaching AdministratorAccess policy..." -ForegroundColor Yellow
aws iam attach-role-policy `
  --role-name GitHubActionsACLRole `
  --policy-arn arn:aws:iam::aws:policy/AdministratorAccess
Write-Host "Policy attached" -ForegroundColor Green

# Step 5: Get Role ARN
Write-Host "`nStep 5: Getting Role ARN..." -ForegroundColor Yellow
$ROLE_ARN = aws iam get-role --role-name GitHubActionsACLRole --query 'Role.Arn' --output text
Write-Host "Role ARN: $ROLE_ARN" -ForegroundColor Green

# Summary
Write-Host "`n" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host "✅ OIDC Configuration Complete!" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host "Account ID: $ACCOUNT_ID" -ForegroundColor White
Write-Host "OIDC Provider ARN: $OIDC_ARN" -ForegroundColor White
Write-Host "Role ARN: $ROLE_ARN" -ForegroundColor White
Write-Host "`n⭐ Add this to GitHub Secrets:" -ForegroundColor Yellow
Write-Host "Name: AWS_ROLE_TO_ASSUME" -ForegroundColor Cyan
Write-Host "Value: $ROLE_ARN" -ForegroundColor Cyan
Write-Host "`nCleanup:"
Write-Host "rm github-policy.json" -ForegroundColor Gray
