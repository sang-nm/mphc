<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <div class="block-productsDetail">
      <h1 class="title-page">
        <xsl:value-of select="/ProductDetail/ZoneTitle" disable-output-escaping="yes"></xsl:value-of>
      </h1>
      <div class="productsInfoWrap">
        <div class="row">
          <div class="col-xs-12 col-md-6 col-lg-7">
            <xsl:if test="count(/ProductDetail/ProductImages)>0">
              <div class="gallery clearfix">
                <div class="one-itemImg">
                  <xsl:apply-templates select="/ProductDetail/ProductImages"></xsl:apply-templates>
                </div>
              </div>
            </xsl:if>
          </div>
          <div class="col-xs-12 col-md-6 col-lg-5">
            <div class="prd-content">
              <h2>
                <xsl:value-of select="/ProductDetail/Title" disable-output-escaping="yes"></xsl:value-of>
                <xsl:value-of select="/ProductDetail/EditLink" disable-output-escaping="yes"></xsl:value-of>
              </h2>
              <div class="boxWrap">
                <div class="boxInfo">
                  <div class="brief">
                    <xsl:value-of select="/ProductDetail/BriefContent" disable-output-escaping="yes"></xsl:value-of>
                  </div>
                  <div class="code">
                    <xsl:text>Mã sản phẩm: </xsl:text>
                    <xsl:value-of select="/ProductDetail/Code" disable-output-escaping="yes"></xsl:value-of>
                  </div>
                  <div class="fullContent">
                    <xsl:value-of select="/ProductDetail/FullContent" disable-output-escaping="yes"></xsl:value-of>
                  </div>
                  <div class="price">
                    <span>Giá: </span>
                    <span class="price-text">
                      <xsl:value-of select="/ProductDetail/Price" disable-output-escaping="yes"></xsl:value-of>
                    </span>
                  </div>
                  <div class="btn-orderCart">
                    <xsl:if test="/ProductDetail/Price != ''">
                      <div class="prd-option">
                        <div class="quantity variations_button">
                          <label>
                            <xsl:text>Số lượng: </xsl:text>
                          </label>
                          <div class="input-number">
                            <input type="text" value="1" class="input-text">
                              <xsl:attribute name="name">
                                <xsl:text>addtocart_</xsl:text>
                                <xsl:value-of select="/ProductDetail/ProductId" disable-output-escaping="yes"></xsl:value-of>
                                <xsl:text>.EnteredQuantity</xsl:text>
                              </xsl:attribute>
                            </input>
                          </div>
                        </div>
                      </div>
                      <div class="prd-cart-button">
                        <a href="#" class="btn-cart" onclick="AjaxCart.addproducttocart_details(this); return false;">
                          <xsl:attribute name="data-productid">
                            <xsl:value-of select="/ProductDetail/ProductId"></xsl:value-of>
                          </xsl:attribute>
                          <i class="fa fa-shopping-cart" aria-hidden="true"></i>
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="/ProductDetail/OrderBuyTxt" disable-output-escaping="yes"></xsl:value-of>
                        </a>
                      </div>
                    </xsl:if>
                  </div>
                  <div class="social clearfix">
                    <div class="google">
                      <xsl:value-of select="/ProductDetail/PlusOne" disable-output-escaping="yes"></xsl:value-of>
                    </div>
                    <div class="face-like">
                      <xsl:value-of select="/ProductDetail/FacebookLike" disable-output-escaping="yes"></xsl:value-of>
                    </div>
                    <div class="fb-share-button" data-layout="button_count"></div>
                    <div class="tweet">
                      <xsl:value-of select="/ProductDetail/TweetThis" disable-output-escaping="yes"></xsl:value-of>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="prd-desc">
      <div class="row">
        <div class="col-xs-12">
          <!-- Nav tabs -->
          <ul class="nav nav-tabs responsive" role="tablist">
            <xsl:apply-templates select="/ProductDetail/ProductAttributes" mode="tabs"></xsl:apply-templates>
            <li class="nav-item">
              <a class="nav-link" role="tab" data-toggle="tab" href="#tab-0" aria-controls="tab-0">Cảm nhận và đánh giá</a>
            </li>
          </ul>
          <!-- Tab panes -->
          <div class="tab-content responsive">
            <xsl:apply-templates select="/ProductDetail/ProductAttributes" mode="tabcontent"></xsl:apply-templates>
            <div role="tabpanel" class="tab-pane fade" id="tab-0">
              <div class="fb-comments" data-width="100%" data-numposts="5">
                <xsl:attribute name="data-href">
                  <xsl:value-of select="/ProductDetail/FullUrl"></xsl:value-of>
                </xsl:attribute>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <xsl:if test="count(/ProductDetail/ProductImages1)>0">
      <h4>Các khách hàng đã sử dụng</h4>
      <xsl:apply-templates select="/ProductDetail/ProductImages1"></xsl:apply-templates>
    </xsl:if>
    <xsl:if test="count(/ProductDetail/ProductImages2)>0">
      <h4>Hiệu quả khi sử dụng</h4>
      <xsl:apply-templates select="/ProductDetail/ProductImages2"></xsl:apply-templates>
    </xsl:if>

    <xsl:if test="count(/ProductDetail/ProductOther)>0">
      <section class="block-productsMore mrb30">
        <h3>
          <span>
            <xsl:value-of select="/ProductDetail/ProductOtherTxt"></xsl:value-of>
          </span>
        </h3>
        <div class="product-other">
          <div class="row">
            <xsl:apply-templates select="/ProductDetail/ProductOther"></xsl:apply-templates>
          </div>
        </div>
      </section>
    </xsl:if>
  </xsl:template>

  <xsl:template match="ProductAttributes" mode="tabs">
    <li class="nav-item">
      <a class="nav-link" role="tab" data-toggle="tab">
        <xsl:if test="position()=1">
          <xsl:attribute name="class">
            <xsl:text>nav-link active</xsl:text>
          </xsl:attribute>
        </xsl:if>
        <xsl:attribute name="href">
          <xsl:text>#tab-</xsl:text>
          <xsl:value-of select="position()"></xsl:value-of>
        </xsl:attribute>
        <xsl:attribute name="aria-controls">
          <xsl:text>tab-</xsl:text>
          <xsl:value-of select="position()"></xsl:value-of>
        </xsl:attribute>
        <xsl:value-of select="Title"></xsl:value-of>
      </a>
    </li>
  </xsl:template>

  <xsl:template match="ProductAttributes" mode="tabcontent">
    <div role="tabpanel" class="tab-pane fade">
      <xsl:if test="position()=1">
        <xsl:attribute name="class">
          <xsl:text>tab-pane fade in active</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <xsl:attribute name="id">
        <xsl:text>tab-</xsl:text>
        <xsl:value-of select="position()"></xsl:value-of>
      </xsl:attribute>
      <div class="contentFull">
        <div class="contentWrap">
          <xsl:value-of select="Content" disable-output-escaping="yes"></xsl:value-of>
        </div>
      </div>
    </div>
  </xsl:template>

  <xsl:template match="ProductImages">
    <xsl:if test="Type=0">
      <div>
        <a rel="group" class="icon-zoom">
          <xsl:attribute name="href">
            <xsl:value-of select="ImageUrl"></xsl:value-of>
          </xsl:attribute>
          <xsl:attribute name="title">
            <xsl:value-of select="Title"></xsl:value-of>
          </xsl:attribute>
        </a>
        <img>
          <xsl:attribute name="src">
            <xsl:value-of select="ImageUrl"></xsl:value-of>
          </xsl:attribute>
          <xsl:attribute name="alt">
            <xsl:value-of select="Title"></xsl:value-of>
          </xsl:attribute>
        </img>
      </div>
    </xsl:if>
  </xsl:template>

  <xsl:template match="ProductImages1">
    <img>
      <xsl:attribute name="src">
        <xsl:value-of select="ImageUrl"></xsl:value-of>
      </xsl:attribute>
      <xsl:attribute name="alt">
        <xsl:value-of select="Title"></xsl:value-of>
      </xsl:attribute>
    </img>
  </xsl:template>

  <xsl:template match="ProductImages2">
    <div>
      <img>
        <xsl:attribute name="src">
          <xsl:value-of select="ImageUrl"></xsl:value-of>
        </xsl:attribute>
        <xsl:attribute name="alt">
          <xsl:value-of select="Title"></xsl:value-of>
        </xsl:attribute>
      </img>
    </div>
  </xsl:template>

  <xsl:template match="ProductOther">
    <div class="col-xs-6 col-md-6 col-lg-3 mrb30">
      <div class="item-prd">
        <figure>
          <a class="center-block">
            <xsl:attribute name="href">
              <xsl:value-of select="Url"></xsl:value-of>
            </xsl:attribute>
            <xsl:attribute name="target">
              <xsl:value-of select="Target"></xsl:value-of>
            </xsl:attribute>
            <xsl:attribute name="title">
              <xsl:value-of select="Title"></xsl:value-of>
            </xsl:attribute>
            <img>
              <xsl:attribute name="src">
                <xsl:value-of select="ImageUrl"></xsl:value-of>
              </xsl:attribute>
              <xsl:attribute name="alt">
                <xsl:value-of select="Title"></xsl:value-of>
              </xsl:attribute>
            </img>
          </a>
          <figcaption>
            <h3>
              <a>
                <xsl:attribute name="href">
                  <xsl:value-of select="Url"></xsl:value-of>
                </xsl:attribute>
                <xsl:attribute name="target">
                  <xsl:value-of select="Target"></xsl:value-of>
                </xsl:attribute>
                <span>
                  <xsl:value-of select="Title" disable-output-escaping="yes"></xsl:value-of>
                </span>
              </a>
              <xsl:value-of select="EditLink" disable-output-escaping="yes"></xsl:value-of>
            </h3>
            <div class="price">
              <xsl:value-of select="Price" disable-output-escaping="yes"></xsl:value-of>
            </div>
            <a href="#" class="btn-prd">
              <xsl:attribute name="onclick">
                <xsl:text>AjaxCart.addproducttocart_catalog(this);return false;</xsl:text>
              </xsl:attribute>
              <xsl:attribute name="data-productid">
                <xsl:value-of select="ProductId"></xsl:value-of>
              </xsl:attribute>
              <xsl:attribute name="title">
                <xsl:value-of select="Title"></xsl:value-of>
              </xsl:attribute>
              <i class="fa fa-shopping-cart" aria-hidden="true"></i>
              <xsl:text> </xsl:text>
              <xsl:value-of select="/ProductDetail/OrderBuyTxt" disable-output-escaping="yes"></xsl:value-of>
            </a>
          </figcaption>
        </figure>
      </div>
    </div>
  </xsl:template>
</xsl:stylesheet>