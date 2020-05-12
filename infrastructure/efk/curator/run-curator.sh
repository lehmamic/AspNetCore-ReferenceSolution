#!/bin/sh

# dev
echo "Cleanup indices for dev"
/usr/bin/curator_cli --host elasticsearch --port 9200 delete_indices --ignore_empty_list --filter_list '[{"filtertype":"age","source":"name","direction":"older","unit":"days","unit_count":14,"timestring": "%Y.%m.%d"},{"filtertype":"pattern","kind":"prefix","value":"fluentd-dev"}]'

# sit
echo "Cleanup indices for sit"
/usr/bin/curator_cli --host elasticsearch --port 9200 delete_indices --ignore_empty_list --filter_list '[{"filtertype":"age","source":"name","direction":"older","unit":"days","unit_count":14,"timestring": "%Y.%m.%d"},{"filtertype":"pattern","kind":"prefix","value":"fluentd-sit"}]'

# uat
echo "Cleanup indices for uat"
/usr/bin/curator_cli --host elasticsearch --port 9200 delete_indices --ignore_empty_list --filter_list '[{"filtertype":"age","source":"name","direction":"older","unit":"days","unit_count":14,"timestring": "%Y.%m.%d"},{"filtertype":"pattern","kind":"prefix","value":"fluentd-uat"}]'

# prod
echo "Cleanup indices for prod"
/usr/bin/curator_cli --host elasticsearch --port 9200 delete_indices --ignore_empty_list --filter_list '[{"filtertype":"age","source":"name","direction":"older","unit":"days","unit_count":14,"timestring": "%Y.%m.%d"},{"filtertype":"pattern","kind":"prefix","value":"fluentd-prod"}]'