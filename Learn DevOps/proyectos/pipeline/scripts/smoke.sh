#!/usr/bin/env bash
# Smoke test: comprueba que el servicio responde en /health.
set -euo pipefail

base="${1:-http://localhost:8000}"
code="$(curl -s -o /dev/null -w "%{http_code}" "${base}/health")"
echo "GET ${base}/health -> ${code}"
test "${code}" = "200"
