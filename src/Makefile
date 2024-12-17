me := paulgzt

tester:
	docker build -t $(me)/$@ -f Dockerfile .
	helm upgrade -i -f $(HOME)/DYNAMOS/charts/tester/values.yaml tester $(HOME)/DYNAMOS/charts/tester
#	docker push $(me)/$@:latest
