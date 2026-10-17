# helm/harbortasks/templates/_helpers.tpl — Plantillas auxiliares reutilizables
{{- /*
Helpers de nombres y etiquetas estándar de Helm.
- "harbortasks.fullname": nombre de la release (el de los recursos)
- "harbortasks.labels": etiquetas comunes a todos los recursos
Uso en cualquier manifiesto: {{ include "harbortasks.labels" . }}
*/ -}}

{{- define "harbortasks.fullname" -}}
{{- .Release.Name | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "harbortasks.labels" -}}
app.kubernetes.io/name: {{ include "harbortasks.fullname" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/version: {{ .Chart.AppVersion }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end -}}