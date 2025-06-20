#!/usr/bin/env bash
set -euo pipefail

API_URL="https://localhost:5001/api"
EMAIL="admin@lib.com"
PASSWORD="admin123"

echo
echo "1) LOGOWANIE"
resp=$(curl -sk -X POST "$API_URL/auth/login" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"$EMAIL\",\"password\":\"$PASSWORD\"}")

token=$(echo "$resp" | sed -n 's/.*"token":"\([^"]*\)".*/\1/p')
expires=$(echo "$resp" | sed -n 's/.*"expiresInMinutes":\([^,]*\).*/\1/p')

if [[ -z "$token" ]]; then
  echo "❌ Nie udało się zalogować. Odpowiedź:"
  echo "$resp"
  exit 1
fi
echo "✔ Token: $token"
echo "✔ ExpiresInMinutes: $expires"


echo 
echo "2) GET ALL BOOKS"
curl -sk "$API_URL/books" -H "Authorization: Bearer $token" \
     -H "Accept: application/json" | jq .


echo
echo "3) CREATE BOOK"
newId=$(curl -sk -X POST "$API_URL/books" \
  -H "Authorization: Bearer $token" \
  -H "Content-Type: application/json" \
  -d '{"title":"cURL Test","isbn":"000-123","authorId":1,"publishDate":"2025-01-01T00:00:00Z"}' \
  | jq '.id')

echo "✔ Utworzono książkę o id=$newId"


echo
echo "4) GET BY ID"
curl -sk "$API_URL/books/$newId" -H "Authorization: Bearer $token" \
     -H "Accept: application/json" | jq .


echo
echo "5) UPDATE BOOK"
curl -sk -X PUT "$API_URL/books/$newId" \
  -H "Authorization: Bearer $token" \
  -H "Content-Type: application/json" \
  -d "{\"id\":$newId,\"title\":\"cURL Test Updated\",\"isbn\":\"000-1234\",\"authorId\":1,\"publishDate\":\"2025-01-02T00:00:00Z\"}" \
  -w " → HTTP %{http_code}\n"


echo
echo "6) SEARCH BOOK (title contains 'cURL')"
curl -sk "$API_URL/books/search?title=cURL" \
     -H "Authorization: Bearer $token" \
     -H "Accept: application/json" | jq .


echo
echo "7) DELETE BOOK"
curl -sk -X DELETE "$API_URL/books/$newId" \
  -H "Authorization: Bearer $token" \
  -w " → HTTP %{http_code}\n"


echo
echo "WSZYSTKIE TESTY ZAKOŃCZONE POMYŚLNIE ✅"
