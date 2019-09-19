<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <section class="check-out clearfix">
      <div class="table-responsive">
        <table class="table table-hover">
          <thead>
            <tr>
              <th>
                <xsl:value-of select="/ShoppingCart/ProductCodeText"></xsl:value-of>
              </th>
              <th>
                <xsl:value-of select="/ShoppingCart/ImageText"></xsl:value-of>
              </th>
              <th>
                <xsl:value-of select="/ShoppingCart/ProductText"></xsl:value-of>
              </th>
              <th>
                <xsl:value-of select="/ShoppingCart/QuantityText"></xsl:value-of>
              </th>
              <th>
                <xsl:value-of select="/ShoppingCart/RemoveText"></xsl:value-of>
              </th>
            </tr>
          </thead>
          <tbody>
            <xsl:apply-templates select="/ShoppingCart/CartItem"></xsl:apply-templates>
          </tbody>
        </table>
      </div>
    </section>
  </xsl:template>
  
  <xsl:template match="CartItem">
    <tr>
      <td>
        <div class="prd-code">
          <xsl:value-of select="Code"></xsl:value-of>
        </div>
      </td>
      <td>
        <a>
          <xsl:attribute name="href">
            <xsl:value-of select="Url"></xsl:value-of>
          </xsl:attribute>
          <xsl:attribute name="target">
            <xsl:value-of select="Target"></xsl:value-of>
          </xsl:attribute>
          <img>
            <xsl:attribute name="src">
              <xsl:value-of select="ThumbnailUrl"></xsl:value-of>
            </xsl:attribute>
            <xsl:attribute name="alt">
              <xsl:value-of select="Title"></xsl:value-of>
            </xsl:attribute>
          </img>
        </a>
      </td>
      <td>
        <div class="prd-name">
          <a class="product-name">
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
        </div>
      </td>
      <td>
        <div class="prd-quantity">
          <input type="text" maxlength="5" class="quotation-quantity">
            <xsl:attribute name="data-productid">
              <xsl:value-of select="ProductId"></xsl:value-of>
            </xsl:attribute>
            <xsl:attribute name="value">
              <xsl:value-of select="Quantity"></xsl:value-of>
            </xsl:attribute>
          </input>
        </div>
      </td>
      <td>
        <a class="quotation-remove" href="#">
          <xsl:attribute name="data-productid">
            <xsl:value-of select="ProductId"></xsl:value-of>
          </xsl:attribute>
          <i class="fa fa-trash-o"></i>
        </a>
      </td>
    </tr>
  </xsl:template>
  
</xsl:stylesheet>