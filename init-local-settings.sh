echo "🟢 Setting up local.settings.json ..."

# If file already exists, skip
if [ -f "local.settings.json" ]; then
  echo "✅ local.settings.json already exists. Skipping."
  exit 0
fi

if [ ! -f local.settings.json ]; then
  echo "📄 Copying from template ..."
  cp local.settings.template.json local.settings.json
fi

echo "✅ Done!"