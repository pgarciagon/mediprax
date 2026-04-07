#!/bin/bash
# MediPrax PostgreSQL Backup Script
# Usage: ./scripts/backup.sh [daily|weekly]
# Cron: 0 2 * * * /path/to/mediprax/scripts/backup.sh daily
# Cron: 0 3 * * 0 /path/to/mediprax/scripts/backup.sh weekly

set -euo pipefail

BACKUP_DIR="${BACKUP_DIR:-/var/backups/mediprax}"
DB_HOST="${DB_HOST:-localhost}"
DB_PORT="${DB_PORT:-5432}"
DB_NAME="${DB_NAME:-mediprax}"
DB_USER="${DB_USER:-mediprax}"
RETENTION_DAILY=7
RETENTION_WEEKLY=4

TYPE="${1:-daily}"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
FILENAME="mediprax_${TYPE}_${TIMESTAMP}.sql.gz"

mkdir -p "${BACKUP_DIR}/${TYPE}"

echo "[$(date)] Starting ${TYPE} backup..."

pg_dump -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" \
    --no-owner --no-privileges --clean --if-exists \
    | gzip > "${BACKUP_DIR}/${TYPE}/${FILENAME}"

SIZE=$(du -h "${BACKUP_DIR}/${TYPE}/${FILENAME}" | cut -f1)
echo "[$(date)] Backup complete: ${FILENAME} (${SIZE})"

# Cleanup old backups
if [ "$TYPE" = "daily" ]; then
    find "${BACKUP_DIR}/daily" -name "*.sql.gz" -mtime +${RETENTION_DAILY} -delete
    echo "[$(date)] Cleaned daily backups older than ${RETENTION_DAILY} days"
elif [ "$TYPE" = "weekly" ]; then
    find "${BACKUP_DIR}/weekly" -name "*.sql.gz" -mtime +$((RETENTION_WEEKLY * 7)) -delete
    echo "[$(date)] Cleaned weekly backups older than ${RETENTION_WEEKLY} weeks"
fi

echo "[$(date)] Done."
