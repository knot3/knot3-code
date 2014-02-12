CODE_DIR = src
TEST_DIR = tests

.PHONY: project_code

project_code:
	$(MAKE) -C $(CODE_DIR)
clean:
	$(MAKE) -C $(CODE_DIR) clean
all:
	$(MAKE) -C $(CODE_DIR) all
build:
	$(MAKE) -C $(CODE_DIR) build
install:
	$(MAKE) -C $(CODE_DIR) install
uninstall:
	$(MAKE) -C $(CODE_DIR) uninstall
distclean:
	$(MAKE) -C $(CODE_DIR) distclean
test:
	$(MAKE) -C $(CODE_DIR) build
	$(MAKE) -C $(TEST_DIR) test
