#!/bin/bash

# Exit on error
set -e

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Check if entity name is provided
if [ -z "$1" ]; then
    echo -e "${RED}Error: Entity name is required${NC}"
    echo "Usage: ./generate-entity.sh EntityName"
    exit 1
fi

ENTITY_NAME=$1
TEMPLATE_PATH="$(dirname "$0")/../templates/entity.template"
OUTPUT_PATH="$(dirname "$0")/../Modulith.NewModule.Domain/Entities/${ENTITY_NAME}.cs"

# Check if template exists
if [ ! -f "$TEMPLATE_PATH" ]; then
    echo -e "${RED}Error: Template file not found at $TEMPLATE_PATH${NC}"
    exit 1
fi

# Create entity file from template
echo -e "${GREEN}Generating entity $ENTITY_NAME...${NC}"
sed "s/{{EntityName}}/$ENTITY_NAME/g" "$TEMPLATE_PATH" > "$OUTPUT_PATH"

echo -e "${GREEN}Entity $ENTITY_NAME generated successfully at $OUTPUT_PATH${NC}" 