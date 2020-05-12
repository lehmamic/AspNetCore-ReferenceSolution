FROM fluent/fluentd:v1.9-1
USER root
# https://docs.fluentd.org/output/elasticsearch
RUN ["gem", "install", "fluent-plugin-elasticsearch", "--no-rdoc", "--no-ri"]
USER fluent
ENTRYPOINT ["fluentd", "-c", "/fluentd/etc/fluent.conf"]